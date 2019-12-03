using System;
using System.Collections.Generic;

namespace LambdaCalculus
{
    public sealed class Context
    {
        private readonly Context? parent;
        private Dictionary<string, Term>? boundTerms;

        public Context() =>
            boundTerms = new Dictionary<string, Term>();
        private Context(Context parent) =>
            this.parent = parent;

        public Context NewScope() =>
            new Context(this);

        public void AddBoundTerm(string identity, Term term)
        {
            if (boundTerms == null)
            {
                boundTerms = new Dictionary<string, Term>();
            }
            boundTerms[identity] = term;
        }

        public Term? GetBoundTerm(string identity)
        {
            Context? current = this;
            do
            {
                if (current.boundTerms != null)
                {
                    if (current.boundTerms.TryGetValue(identity, out var term))
                    {
                        return term;
                    }
                }
                current = current.parent;
            }
            while (current != null);

            return null;
        }
    }

    ////////////////////////////////////////////////////////////

    public abstract class Term
    {
        public abstract Term HigherOrder { get; }

        public abstract Term Reduce(Context context);

        public abstract Term Infer(Context context);

        //////////////////////////////////

        private static readonly Dictionary<Type, IdentityTerm> types =
            new Dictionary<Type, IdentityTerm>();

        private static readonly IdentityTerm kind =
            new IdentityTerm("*", UnspecifiedTerm.Instance);

        public static IdentityTerm Identity(Type type)
        {
            if (!types.TryGetValue(type, out var term))
            {
                term = new IdentityTerm(type.FullName, kind);
                types.Add(type, term);
            }
            return term;
        }

        public static IdentityTerm Identity(string identity) =>
            new IdentityTerm(identity, UnspecifiedTerm.Instance);

        public static IdentityTerm Kind() =>
            kind;

        public static BooleanTerm True() =>
            BooleanTerm.True;
        public static BooleanTerm False() =>
            BooleanTerm.False;

        public static BooleanTerm Constant(bool value) =>
            value ? BooleanTerm.True : BooleanTerm.False;

        public static Term Constant(object value) =>
            new ConstantTerm(value);

        public static ApplyTerm Apply(Term function, Term argument) =>
            new ApplyTerm(function, argument);

        public static Term Lambda(string parameter, Term body) =>
            new ApplyTerm(new ApplyTerm(LambdaArrowTerm.Instance, Identity(parameter)), body);

        public static AndTerm And(Term lhs) =>
            new AndTerm(lhs);
        public static Term And(Term lhs, Term rhs) =>
            new ApplyTerm(new AndTerm(lhs), rhs);

        public static Term If(Term condition, Term then, Term els) =>
            new ApplyTerm(new ApplyTerm(new IfTerm(condition), then), els);
    }

    public sealed class UnspecifiedTerm : Term
    {
        private UnspecifiedTerm()
        { }

        public override Term HigherOrder =>
            null!;

        public override Term Reduce(Context context) =>
            this;

        public override Term Infer(Context context) =>
            this;

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }

    ////////////////////////////////////////////////////////////

    public sealed class IdentityTerm : Term
    {
        public new readonly string Identity;

        internal IdentityTerm(string identity, Term higherOrder)
        {
            this.Identity = identity;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public override Term Reduce(Context context) =>
            context.GetBoundTerm(this.Identity) is Term term ?
                term.Reduce(context) :
                this;

        public override Term Infer(Context context) =>
            context.GetBoundTerm(this.Identity) is Term term ?
                term.Infer(context) :
                this;
    }

    public sealed class ConstantTerm : Term
    {
        public readonly object Value;

        internal ConstantTerm(object value) =>
            this.Value = value;

        public override Term HigherOrder =>
            Identity(this.Value.GetType());

        public override Term Reduce(Context context) =>
            this;

        public override Term Infer(Context context) =>
            this;
    }

    public abstract class ApplicableTerm : Term
    {
        protected internal abstract Term? Apply(Context context, Term rhs);
    }

    public sealed class ApplyTerm : Term
    {
        public readonly Term Function;
        public readonly Term Argument;

        internal ApplyTerm(Term function, Term argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override Term HigherOrder =>
            this.Argument.HigherOrder;

        public override Term Reduce(Context context)
        {
            var function = this.Function.Reduce(context);
            var argument = this.Argument.Reduce(context);

            if (function is ApplicableTerm applicable &&
                applicable.Apply(context, argument) is Term term)
            {
                return term;
            }
            else
            {
                return new ApplyTerm(function, argument);
            }
        }

        public override Term Infer(Context context)
        {
            var function = this.Function.Infer(context);
            var argument = this.Argument.Infer(context);

            return new ApplyTerm(function, argument);
        }
    }

    public sealed class LambdaArrowTerm : ApplicableTerm
    {
        private LambdaArrowTerm()
        {
        }

        public override Term HigherOrder =>
            Lambda("->", Lambda("?", UnspecifiedTerm.Instance));

        public override Term Reduce(Context context) =>
            this;

        protected internal override Term? Apply(Context context, Term rhs) =>
            new LambdaArrowLeftTerm(((IdentityTerm)rhs).Identity);

        public override Term Infer(Context context) =>
            this;

        private sealed class LambdaArrowLeftTerm : ApplicableTerm
        {
            public readonly string Parameter;

            public LambdaArrowLeftTerm(string parameter) =>
                this.Parameter = parameter;

            public override Term HigherOrder =>
                Lambda("?", UnspecifiedTerm.Instance);

            public override Term Reduce(Context context) =>
                this;

            protected internal override Term? Apply(Context context, Term rhs) =>
                new LambdaTerm(this.Parameter, rhs);

            public override Term Infer(Context context) =>
                this;
        }

        public static LambdaArrowTerm Instance =
            new LambdaArrowTerm();
    }

    public sealed class LambdaTerm : ApplicableTerm
    {
        public readonly string Parameter;
        public readonly Term Body;

        internal LambdaTerm(string parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        public override Term HigherOrder =>
            Body.HigherOrder;

        public override Term Reduce(Context context) =>
            new LambdaTerm(this.Parameter, this.Body.Reduce(context));

        protected internal override Term? Apply(Context context, Term rhs)
        {
            var newScope = context.NewScope();
            newScope.AddBoundTerm(this.Parameter, rhs);

            return this.Body.Reduce(newScope);
        }

        public override Term Infer(Context context) =>
            new LambdaTerm(this.Parameter, this.Body.Infer(context));
    }

    ////////////////////////////////////////////////////////////

    public sealed class BooleanTerm : Term
    {
        internal static readonly IdentityTerm higherOrder =
            Identity(typeof(bool));

        public readonly bool Value;

        private BooleanTerm(bool value) =>
            this.Value = value;

        public override Term HigherOrder =>
            higherOrder;

        public override Term Reduce(Context context) =>
            this;

        public override Term Infer(Context context) =>
            this;

        public static new readonly BooleanTerm True =
            new BooleanTerm(true);
        public static new readonly BooleanTerm False =
            new BooleanTerm(false);
    }

    public sealed class AndTerm : ApplicableTerm
    {
        public readonly Term Lhs;

        internal AndTerm(Term lhs) =>
            this.Lhs = lhs;

        public override Term HigherOrder =>
           BooleanTerm.higherOrder;

        public override sealed Term Reduce(Context context) =>
            new AndTerm(this.Lhs.Reduce(context));

        public override Term Infer(Context context) =>
            new AndTerm(this.Lhs.Infer(context));

        protected internal override Term? Apply(Context context, Term rhs) =>
            (this.Lhs is BooleanTerm l && rhs is BooleanTerm r) ?
                Term.Constant(l.Value && r.Value) :
                null;
    }

    public sealed class IfTerm : ApplicableTerm
    {
        public readonly Term Condition;

        internal IfTerm(Term condition) =>
            this.Condition = condition;

        public override Term HigherOrder =>
           UnspecifiedTerm.Instance;

        public override sealed Term Reduce(Context context) =>
            new IfTerm(this.Condition.Reduce(context));

        protected internal override Term? Apply(Context context, Term rhs) =>
            (this.Condition is BooleanTerm c) ?
                (c.Value ? (Term)new ThenTerm(rhs) : ElseTerm.Instance) :
                null;

        public override Term Infer(Context context) =>
            new IfTerm(this.Condition.Infer(context));

        private sealed class ThenTerm : ApplicableTerm
        {
            public readonly Term Then;

            public ThenTerm(Term then) =>
                this.Then = then;

            public override Term HigherOrder =>
                this.Then.HigherOrder;

            public override Term Reduce(Context context) =>
                this;

            protected internal override Term? Apply(Context context, Term rhs) =>
                this.Then;

            public override Term Infer(Context context) =>
                this;
        }

        private sealed class ElseTerm : ApplicableTerm
        {
            private ElseTerm()
            { }

            public override Term HigherOrder =>
                UnspecifiedTerm.Instance;

            public override Term Reduce(Context context) =>
                this;

            protected internal override Term? Apply(Context context, Term rhs) =>
                rhs;

            public override Term Infer(Context context) =>
                this;

            public static readonly ElseTerm Instance =
                new ElseTerm();
        }
    }
}
