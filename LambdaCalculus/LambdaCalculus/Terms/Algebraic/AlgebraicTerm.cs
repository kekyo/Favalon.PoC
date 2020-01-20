using Favalon.Contexts;

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
                this.Lhs.Equals(lhs, true) &&
                this.Rhs.Equals(rhs, true) &&
                this.HigherOrder.Equals(higherOrder, true) ?
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

        protected override bool OnEquals(Term? other) =>
            other is T term ?
                this.Lhs.Equals(term.Lhs) && this.Rhs.Equals(term.Rhs) :
                false;
    }
}
