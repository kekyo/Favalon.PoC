namespace Favalon.Terms.Operators
{
    public abstract class BinaryOperatorTerm : Term
    {
        public readonly Term Lhs;
        public readonly Term Rhs;

        internal BinaryOperatorTerm(Term lhs, Term rhs)
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        protected abstract Term Create(Term lhs, Term rhs, Term higherOrder);

        protected virtual Term Infer(InferContext context, Term lhs, Term rhs, Term higherOrder) =>
            this.Create(lhs, rhs, higherOrder);

        public override sealed Term Infer(InferContext context)
        {
            var lhs = this.Lhs.Infer(context);
            var rhs = this.Rhs.Infer(context);
            var higherOrder = this.HigherOrder.Infer(context);

            context.Unify(lhs.HigherOrder, higherOrder);
            context.Unify(rhs.HigherOrder, higherOrder);

            return this.Infer(context, lhs, rhs, higherOrder);
        }

        protected virtual Term Fixup(FixupContext context, Term lhs, Term rhs, Term higherOrder) =>
            this.Create(lhs, rhs, higherOrder);

        public override sealed Term Fixup(FixupContext context) =>
            this.Fixup(context, this.Lhs.Fixup(context), this.Rhs.Fixup(context), this.HigherOrder.Fixup(context));

        public override int GetHashCode() =>
            this.Lhs.GetHashCode() ^ this.Rhs.GetHashCode();
    }

    public abstract class BinaryOperatorTerm<T> : BinaryOperatorTerm
        where T : BinaryOperatorTerm
    {
        protected BinaryOperatorTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T rhs ?
                (this.Lhs.Equals(rhs.Lhs) && this.Rhs.Equals(rhs.Rhs)) :
                false;
    }

    public abstract class LogicalBinaryOperatorTerm : BinaryOperatorTerm
    {
        internal LogicalBinaryOperatorTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        public override sealed Term HigherOrder =>
            BooleanTerm.Type;
    }

    public abstract class LogicalBinaryOperatorTerm<T> : LogicalBinaryOperatorTerm
        where T : BinaryOperatorTerm
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
