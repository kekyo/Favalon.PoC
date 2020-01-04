namespace Favalon.Terms.Logical
{
    public abstract class LogicalBinaryTerm : BinaryTerm
    {
        internal LogicalBinaryTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        public override sealed Term HigherOrder =>
            BooleanTerm.Type;
    }

    public abstract class LogicalBinaryTerm<T> : LogicalBinaryTerm
        where T : BinaryTerm
    {
        protected LogicalBinaryTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T rhs ?
                (this.Lhs.Equals(rhs.Lhs) && this.Rhs.Equals(rhs.Rhs)) :
                false;
    }
}
