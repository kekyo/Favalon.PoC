using System;

namespace LambdaCalculus
{
    public abstract class Term
    {
        public abstract Term Reduce();
    }

    public sealed class BooleanTerm : Term
    {
        public readonly bool Value;

        public BooleanTerm(bool value) =>
            this.Value = value;

        public override Term Reduce() =>
            this;
    }

    public sealed class ApplyTerm : Term
    {
        public readonly Term Function;
        public readonly Term Argument;

        public ApplyTerm(Term function, Term argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override Term Reduce() =>
            ((CallableTerm)this.Function.Reduce()).Call(this.Argument.Reduce());
    }

    public abstract class CallableTerm : Term
    {
        public abstract Term Call(Term argument);
    }

    public sealed class AndTerm : CallableTerm
    {
        public readonly Term Lhs;

        public AndTerm(Term lhs) =>
            this.Lhs = lhs;

        public override Term Reduce() =>
            new AndTerm(this.Lhs.Reduce());

        public override Term Call(Term rhs) =>
            new BooleanTerm(((BooleanTerm)this.Lhs).Value && ((BooleanTerm)rhs).Value);
    }
}
