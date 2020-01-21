using Favalon.TermContexts;

namespace Favalon.Terms.Algebraic
{
    public abstract class AlgebraicTerm : BinaryTerm
    {
        protected AlgebraicTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }

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
        protected AlgebraicTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is T term ?
                this.Lhs.Equals(context, term.Lhs) && this.Rhs.Equals(context, term.Rhs) :
                false;
    }
}
