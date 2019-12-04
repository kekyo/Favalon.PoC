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

        protected abstract void Infer(InferContext context, Term lhs, Term rhs);

        public override sealed Term Infer(InferContext context)
        {
            var lhs = this.Lhs.Infer(context);
            var rhs = this.Rhs.Infer(context);

            this.Infer(context, lhs, rhs);

            return this.Create(lhs, rhs);
        }

        public override sealed Term Fixup(InferContext context) =>
            this.Create(this.Lhs.Fixup(context), this.Rhs.Fixup(context));
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
            BooleanTerm.higherOrder;

        protected override sealed void Infer(InferContext context, Term lhs, Term rhs)
        {
            context.Unify(lhs.HigherOrder, BooleanTerm.higherOrder);
            context.Unify(rhs.HigherOrder, BooleanTerm.higherOrder);
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

    ///////////////////////////////////////////////////////////////////////

    public abstract class OperatorSymbolTerm : ApplicableTerm
    {
        internal OperatorSymbolTerm()
        { }

        public override sealed Term HigherOrder =>
            UnspecifiedTerm.Instance;

        protected abstract Term Create(Term argument);

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        protected internal override sealed Term? ReduceForApply(ReduceContext context, Term rhs) =>
            this.Create(rhs);

        public override sealed Term Infer(InferContext context) =>
            this;

        public override sealed Term Fixup(InferContext context) =>
            this;
    }

    public abstract class OperatorSymbolTerm<T> : OperatorSymbolTerm
        where T : Term
    {
        protected OperatorSymbolTerm()
        { }

        public override sealed bool Equals(Term? other) =>
            other is T;
    }

    public abstract class OperatorArgumentTerm : ApplicableTerm
    {
        public readonly Term Argument;

        internal OperatorArgumentTerm(Term argument) =>
            this.Argument = argument;

        public override sealed Term HigherOrder =>
            UnspecifiedTerm.Instance;

        protected abstract Term Create(Term argument);

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        public override sealed Term Infer(InferContext context) =>
            this.Create(this.Argument.Infer(context));

        public override sealed Term Fixup(InferContext context) =>
            this.Create(this.Argument.Fixup(context));
    }

    public abstract class OperatorArgumentTerm<T> : OperatorArgumentTerm
        where T : OperatorArgumentTerm
    {
        protected OperatorArgumentTerm(Term argument) :
            base(argument)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T rhs ? this.Argument.Equals(rhs.Argument) : false;
    }
}
