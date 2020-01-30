using Favalon.Terms.Contexts;
using System;

namespace Favalon.Terms.Algebraic
{
    public abstract class AlgebraicTerm : BinaryTerm
    {
        protected readonly AlgebraicCalculator Calculator;

        protected AlgebraicTerm(Term lhs, Term rhs, AlgebraicCalculator calculator) :
            base(lhs, rhs) =>
            this.Calculator = calculator;

        protected static Term GetHigherOrderTerm(Term lhs, Term rhs, Func<Term, Term, Term, Term> onCreate) =>
            (lhs.HigherOrder, rhs.HigherOrder) switch
            {
                (TerminationTerm _, TerminationTerm _) => TerminationTerm.Instance,
                (_, TerminationTerm _) => lhs.HigherOrder,
                (TerminationTerm _, _) => rhs.HigherOrder,
                _ => onCreate(lhs.HigherOrder, rhs.HigherOrder, UnspecifiedTerm.Instance)
            };

        public override Term Reduce(ReduceContext context)
        {
            var lhs = this.Lhs.Reduce(context);
            var rhs = this.Rhs.Reduce(context);
            var higherOrder = this.HigherOrder.Reduce(context);

            return
                this.Lhs.EqualsWithHigherOrder(lhs) &&
                this.Rhs.EqualsWithHigherOrder(rhs) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    this.OnCreate(lhs, rhs, higherOrder);
        }
    }

    public abstract class AlgebraicTerm<T> : AlgebraicTerm
        where T : AlgebraicTerm
    {
        protected AlgebraicTerm(Term lhs, Term rhs, AlgebraicCalculator calculator) :
            base(lhs, rhs, calculator)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is T term ?
                this.Lhs.Equals(context, term.Lhs) && this.Rhs.Equals(context, term.Rhs) :
                false;
    }
}
