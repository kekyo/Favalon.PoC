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
            BooleanTerm.Type;

        protected override sealed void Infer(InferContext context, Term lhs, Term rhs)
        {
            context.Unify(lhs.HigherOrder, BooleanTerm.Type);
            context.Unify(rhs.HigherOrder, BooleanTerm.Type);
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

    public abstract class OperatorSymbolTerm : Term
    {
        internal OperatorSymbolTerm()
        { }

        public override Term HigherOrder =>
            LambdaCalculus.UnspecifiedTerm.Instance;

        public override sealed Term Reduce(ReduceContext context) =>
            this;

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

    public abstract class OperatorArgument0Term : Term
    {
        public readonly Term Argument0;

        internal OperatorArgument0Term(Term argument0) =>
            this.Argument0 = argument0;

        public override Term HigherOrder =>
            LambdaCalculus.UnspecifiedTerm.Instance;

        protected abstract Term Create(Term argument0);

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        public override sealed Term Infer(InferContext context) =>
            this.Create(this.Argument0.Infer(context));

        public override sealed Term Fixup(InferContext context) =>
            this.Create(this.Argument0.Fixup(context));
    }

    public abstract class OperatorArgument0Term<T> : OperatorArgument0Term
        where T : OperatorArgument0Term
    {
        protected OperatorArgument0Term(Term argument) :
            base(argument)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T rhs ? this.Argument0.Equals(rhs.Argument0) : false;
    }

    public abstract class OperatorArgument1Term : Term
    {
        public readonly Term Argument0;
        public readonly Term Argument1;

        internal OperatorArgument1Term(Term argument0, Term argument1)
        {
            this.Argument0 = argument0;
            this.Argument1 = argument1;
        }

        public override Term HigherOrder =>
            LambdaCalculus.UnspecifiedTerm.Instance;

        protected abstract Term Create(Term argument0, Term argument1);

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        public override sealed Term Infer(InferContext context) =>
            this.Create(this.Argument0.Infer(context), this.Argument1.Infer(context));

        public override sealed Term Fixup(InferContext context) =>
            this.Create(this.Argument0.Fixup(context), this.Argument1.Fixup(context));
    }

    public abstract class OperatorArgument1Term<T> : OperatorArgument1Term
        where T : OperatorArgument1Term
    {
        protected OperatorArgument1Term(Term argument0, Term argument1) :
            base(argument0, argument1)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T rhs ?
                (this.Argument0.Equals(rhs.Argument0) && this.Argument1.Equals(rhs.Argument1)) :
                false;
    }
}
