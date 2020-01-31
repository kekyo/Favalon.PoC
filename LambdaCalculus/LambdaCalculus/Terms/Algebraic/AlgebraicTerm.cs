using Favalon.Terms.Contexts;

namespace Favalon.Terms.Algebraic
{
    public abstract class AlgebraicTerm : BinaryTerm
    {
        protected readonly AlgebraicCalculator Calculator;

        protected AlgebraicTerm(Term lhs, Term rhs, Term higherOrder, AlgebraicCalculator calculator) :
            base(lhs, rhs, higherOrder) =>
            this.Calculator = calculator;

        public override Term Infer(InferContext context)
        {
            var lhs = this.Lhs.Infer(context);
            var rhs = this.Rhs.Infer(context);
            var higherOrder = context.ResolveHigherOrder(this.HigherOrder);

            var calculatedHigherOrder = this.OnCreate(this.Lhs.HigherOrder, this.Rhs.HigherOrder, UnspecifiedTerm.Instance);
            var calculatedHigherOrder_ = context.ResolveHigherOrder(calculatedHigherOrder);

            context.Unify(lhs.HigherOrder, higherOrder);
            context.Unify(rhs.HigherOrder, higherOrder);
            context.Unify(calculatedHigherOrder_, higherOrder);

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
        protected AlgebraicTerm(Term lhs, Term rhs, Term higherOrder, AlgebraicCalculator calculator) :
            base(lhs, rhs, higherOrder, calculator)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is T term ?
                this.Lhs.Equals(context, term.Lhs) && this.Rhs.Equals(context, term.Rhs) :
                false;
    }
}
