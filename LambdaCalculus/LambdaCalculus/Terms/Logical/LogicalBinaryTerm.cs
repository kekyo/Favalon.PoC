namespace Favalon.Terms.Logical
{
    public abstract class LogicalBinaryTerm : BinaryTerm
    {
        protected LogicalBinaryTerm(Term lhs, Term rhs) :
            base(lhs, rhs, BooleanTerm.Type)
        { }

        protected abstract Term OnCreate(Term lhs, Term rhs);

        protected override sealed Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            this.OnCreate(lhs, rhs);
    }

    public abstract class LogicalBinaryTerm<T> : LogicalBinaryTerm
        where T : LogicalBinaryTerm
    {
        protected LogicalBinaryTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        protected override bool OnEquals(Term? other) =>
            other is T term ?
                this.Lhs.Equals(term.Lhs) && this.Rhs.Equals(term.Rhs) :
                false;
    }
}
