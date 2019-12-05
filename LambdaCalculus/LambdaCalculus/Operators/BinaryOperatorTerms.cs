namespace LambdaCalculus.Operators
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

        protected abstract Term Create(Term lhs, Term rhs);

        protected virtual Term Infer(InferContext context, Term lhs, Term rhs) =>
            this.Create(lhs, rhs);

        public override sealed Term Infer(InferContext context) =>
            this.Infer(context, this.Lhs.Infer(context), this.Rhs.Infer(context));

        protected virtual Term Fixup(FixupContext context, Term lhs, Term rhs) =>
            this.Create(lhs, rhs);

        public override sealed Term Fixup(FixupContext context) =>
            this.Fixup(context, this.Lhs.Fixup(context), this.Rhs.Fixup(context));

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

        protected override sealed Term Infer(InferContext context, Term lhs, Term rhs)
        {
            context.Unify(lhs.HigherOrder, BooleanTerm.Type);
            context.Unify(rhs.HigherOrder, BooleanTerm.Type);

            return this.Create(lhs, rhs);
        }
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
