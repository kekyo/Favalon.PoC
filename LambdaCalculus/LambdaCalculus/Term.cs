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

        //////////////////////////////////

        protected static class IdentityGenerator<T>
        {
            public static readonly IdentityTerm Instance =
                new IdentityTerm(typeof(T).FullName);
        }

        public static IdentityTerm Identity(string identity) =>
            new IdentityTerm(identity);

        public static BooleanTerm True() =>
            BooleanTerm.True;
        public static BooleanTerm False() =>
            BooleanTerm.False;

        public static BooleanTerm Constant(bool value) =>
            value ? BooleanTerm.True : BooleanTerm.False;

        public static ApplyTerm Apply(Term function, Term argument) =>
            new ApplyTerm(function, argument);

        public static LambdaTerm Lambda(string parameter, Term body) =>
            new LambdaTerm(parameter, body);

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

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }

    ////////////////////////////////////////////////////////////

    public sealed class IdentityTerm : Term
    {
        public new readonly string Identity;

        internal IdentityTerm(string identity) =>
            this.Identity = identity;

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        public override Term Reduce(Context context) =>
            context.GetBoundTerm(this.Identity) is Term term ?
                term :
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
            UnspecifiedTerm.Instance;

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
    }

    public sealed class LambdaArrowTerm : ApplicableTerm
    {
        private LambdaArrowTerm()
        {
        }

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        public override Term Reduce(Context context) =>
            this;

        protected internal override Term? Apply(Context context, Term rhs) =>
            new LambdaArrowLeftTerm(((IdentityTerm)rhs).Identity);

        private sealed class LambdaArrowLeftTerm : ApplicableTerm
        {
            public readonly string Parameter;

            public LambdaArrowLeftTerm(string parameter) =>
                this.Parameter = parameter;

            public override Term HigherOrder =>
                UnspecifiedTerm.Instance;

            public override Term Reduce(Context context) =>
                this;

            protected internal override Term? Apply(Context context, Term rhs) =>
                Term.Lambda(this.Parameter, rhs);
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
            UnspecifiedTerm.Instance;

        public override Term Reduce(Context context) =>
            new LambdaTerm(this.Parameter, this.Body.Reduce(context));

        protected internal override Term? Apply(Context context, Term rhs)
        {
            var newScope = context.NewScope();
            newScope.AddBoundTerm(this.Parameter, rhs);

            return this.Body.Reduce(newScope);
        }
    }

    ////////////////////////////////////////////////////////////

    public sealed class BooleanTerm : Term
    {
        public readonly bool Value;

        private BooleanTerm(bool value) =>
            this.Value = value;

        public override Term HigherOrder =>
            IdentityGenerator<bool>.Instance;

        public override Term Reduce(Context context) =>
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
           IdentityGenerator<bool>.Instance;

        public override sealed Term Reduce(Context context) =>
            new AndTerm(this.Lhs.Reduce(context));

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

        private sealed class ThenTerm : ApplicableTerm
        {
            public readonly Term Then;

            public ThenTerm(Term then) =>
                this.Then = then;

            public override Term HigherOrder =>
                UnspecifiedTerm.Instance;

            public override Term Reduce(Context context) =>
                this;

            protected internal override Term? Apply(Context context, Term rhs) =>
                this.Then;
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

            public static readonly ElseTerm Instance =
                new ElseTerm();
        }
    }
}
