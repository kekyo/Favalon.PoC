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

        public static LambdaTerm Lambda(string parameter, Term body) =>
            new LambdaTerm(parameter, body);

        public static AndTerm And(Term lhs, Term rhs) =>
            new AndTerm(lhs, rhs);

        public static Term If(Term condition, Term then, Term els) =>
            new IfTerm(condition, then, els);
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

            if (function is ApplicableTerm applicable &&
                applicable.Apply(context, this.Argument) is Term term)
            {
                return term;
            }
            else
            {
                return new ApplyTerm(function, this.Argument.Reduce(context));
            }
        }

        public override Term Infer(Context context)
        {
            var function = this.Function.Infer(context);
            var argument = this.Argument.Infer(context);

            return new ApplyTerm(function, argument);
        }
    }

    public sealed class LambdaOPeratorTerm : ApplicableTerm
    {
        private LambdaOPeratorTerm()
        { }

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        public override Term Reduce(Context context) =>
            this;

        protected internal override Term? Apply(Context context, Term rhs) =>
            new LambdaArrowParameterTerm(((IdentityTerm)rhs.Reduce(context)).Identity);

        public override Term Infer(Context context) =>
            this;

        private sealed class LambdaArrowParameterTerm : ApplicableTerm
        {
            public readonly string Parameter;

            public LambdaArrowParameterTerm(string parameter) =>
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

        public static LambdaOPeratorTerm Instance =
            new LambdaOPeratorTerm();
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
            this.Body.HigherOrder;

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

    public sealed class AndOperatorTerm : ApplicableTerm
    {
        private AndOperatorTerm()
        { }

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        public override sealed Term Reduce(Context context) =>
            this;

        protected internal override Term? Apply(Context context, Term rhs) =>
            new AndLeftTerm(rhs.Reduce(context));

        public override Term Infer(Context context) =>
            this;

        public static readonly AndOperatorTerm Instance =
            new AndOperatorTerm();

        private sealed class AndLeftTerm : ApplicableTerm
        {
            public readonly Term Lhs;

            public AndLeftTerm(Term lhs) =>
                this.Lhs = lhs;

            public override Term HigherOrder =>
                UnspecifiedTerm.Instance;

            public override Term Reduce(Context context) =>
                this;

            protected internal override Term? Apply(Context context, Term rhs) =>
                AndTerm.Reduce(context, this.Lhs, rhs);

            public override Term Infer(Context context) =>
                new AndLeftTerm(this.Lhs.Infer(context));
        }
    }

    public sealed class AndTerm : Term
    {
        public readonly Term Lhs;
        public readonly Term Rhs;

        internal AndTerm(Term lhs, Term rhs)
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public override Term HigherOrder =>
           BooleanTerm.higherOrder;

        internal static Term Reduce(Context context, Term lhs, Term rhs)
        {
            var lhs_ = lhs.Reduce(context);
            if (lhs_ is BooleanTerm l)
            {
                if (l.Value)
                {
                    var rhs_ = rhs.Reduce(context);
                    if (rhs_ is BooleanTerm r)
                    {
                        return Constant(l.Value && r.Value);
                    }
                    else
                    {
                        return new AndTerm(lhs_, rhs_);
                    }
                }
                else
                {
                    return False();
                }
            }
            else
            {
                return new AndTerm(lhs_, rhs);
            }
        }

        public override sealed Term Reduce(Context context) =>
            Reduce(context, this.Lhs, this.Rhs);

        public override Term Infer(Context context) =>
            new AndTerm(this.Lhs.Infer(context), this.Rhs.Infer(context));
    }

    public sealed class IfOperatorTerm : ApplicableTerm
    {
        private IfOperatorTerm()
        { }

        public override Term HigherOrder =>
           UnspecifiedTerm.Instance;

        public override sealed Term Reduce(Context context) =>
            this;

        protected internal override Term? Apply(Context context, Term rhs) =>
            new ThenTerm(rhs);

        public override Term Infer(Context context) =>
            this;

        public static readonly IfOperatorTerm Instance =
            new IfOperatorTerm();

        private sealed class ThenTerm : ApplicableTerm
        {
            public readonly Term Condition;

            public ThenTerm(Term then) =>
                this.Condition = then;

            public override Term HigherOrder =>
                this.Condition.HigherOrder;

            public override Term Reduce(Context context) =>
                new ThenTerm(this.Condition.Reduce(context));

            protected internal override Term? Apply(Context context, Term rhs) =>
                new ElseTerm(this.Condition, rhs);

            public override Term Infer(Context context) =>
                new ThenTerm(this.Condition.Infer(context));
        }

        private sealed class ElseTerm : ApplicableTerm
        {
            public readonly Term Condition;
            public readonly Term Then;

            public ElseTerm(Term condition, Term then)
            {
                this.Condition = condition;   // Condition term already reduced.
                this.Then = then;
            }

            public override Term HigherOrder =>
                this.Then.HigherOrder;

            public override Term Reduce(Context context) =>
                this;  // Cannot reduce Then term at this time, because has to examine delayed execution at IfTerm.Reduce().

            protected internal override Term? Apply(Context context, Term rhs) =>
                IfTerm.Reduce(context, this.Condition, this.Then, rhs);

            public override Term Infer(Context context) =>
                new ElseTerm(this.Condition, this.Then.Infer(context));
        }
    }

    public sealed class IfTerm : Term
    {
        public readonly Term Condition;
        public readonly Term Then;
        public readonly Term Else;

        internal IfTerm(Term condition, Term then, Term @else)
        {
            this.Condition = condition;
            this.Then = then;
            this.Else = @else;
        }

        public override Term HigherOrder =>
            (this.Condition is BooleanTerm term) ?
                (term.Value ? this.Then.HigherOrder : this.Else.HigherOrder) :
                this.Then.HigherOrder;  // TODO: Unspecified or OrTypes

        internal static Term Reduce(Context context, Term condition, Term then, Term @else) =>
            ((BooleanTerm)condition.Reduce(context)).Value ?
                then.Reduce(context) :   // Reduce only then or else term by the conditional.
                @else.Reduce(context);

        public override Term Reduce(Context context) =>
            Reduce(context, this.Condition, this.Then, this.Else);

        public override Term Infer(Context context) =>
            new IfTerm(this.Condition.Infer(context), this.Then.Infer(context), this.Else.Infer(context));
    }
}
