using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public abstract class LogicalBinaryTerm : BinaryTerm
    {
        protected LogicalBinaryTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }
    }

    public abstract class LogicalBinaryTerm<T> : LogicalBinaryTerm
        where T : LogicalBinaryTerm
    {
        protected LogicalBinaryTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is T term ?
                this.Lhs.Equals(context, term.Lhs) && this.Rhs.Equals(context, term.Rhs) :
                false;
    }
}
