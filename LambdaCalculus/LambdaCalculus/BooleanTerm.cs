using System;

namespace LambdaCalculus
{
    public abstract class Term
    {
        public abstract Term Reduce();

        public abstract Term Call(Term argument);
    }

    public sealed class BooleanTerm : Term
    {
        public readonly bool Value;

        public BooleanTerm(bool value) =>
            this.Value = value;

        public override Term Reduce() =>
            this;

        public override Term Call(Term argument) =>
            throw new InvalidOperationException();
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
            this.Function.Reduce().Call(this.Argument.Reduce());

        public override Term Call(Term argument) =>
            throw new InvalidOperationException();
    }

    public sealed class DelegationTerm : Term
    {
        private readonly Func<Term, Term> runner;

        public DelegationTerm(Func<Term, Term> runner) =>
            this.runner = runner;

        public override Term Reduce() =>
            this;

        public override Term Call(Term argument) =>
            runner(argument);
    }

    public sealed class AndTerm : Term
    {
        public AndTerm()
        { }

        public override Term Reduce() =>
            this;

        public override Term Call(Term argument) =>
            new DelegationTerm(rhs => new BooleanTerm(
                ((BooleanTerm)argument.Reduce()).Value && ((BooleanTerm)rhs.Reduce()).Value));
    }
}
