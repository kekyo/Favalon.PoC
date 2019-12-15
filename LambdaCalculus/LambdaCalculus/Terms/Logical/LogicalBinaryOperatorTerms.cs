namespace Favalon.Terms.Logical
{
    public abstract class LogicalBinaryOperatorTerm : BinaryTerm
    {
        internal LogicalBinaryOperatorTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        public override sealed Term HigherOrder =>
            BooleanTerm.Type;
    }

    public abstract class LogicalBinaryOperatorTerm<T> : LogicalBinaryOperatorTerm
        where T : BinaryTerm
    {
        protected LogicalBinaryOperatorTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T rhs ?
                (this.Lhs.Equals(rhs.Lhs) && this.Rhs.Equals(rhs.Rhs)) :
                false;
    }
}
