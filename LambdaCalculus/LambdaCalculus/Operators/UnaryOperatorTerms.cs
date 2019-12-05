namespace LambdaCalculus.Operators
{
    public abstract class UnaryOperatorTerm : Term
    {
        public readonly Term Argument;

        internal UnaryOperatorTerm(Term argument) =>
            this.Argument = argument;

        protected abstract Term Create(Term argument);

        protected virtual Term Infer(InferContext context, Term argument) =>
            this.Create(argument);

        public override sealed Term Infer(InferContext context) =>
            this.Infer(context, this.Argument.Infer(context));

        protected virtual Term Fixup(FixupContext context, Term argument) =>
            this.Create(argument);

        public override sealed Term Fixup(FixupContext context) =>
            this.Fixup(context, this.Argument.Fixup(context));

        public override int GetHashCode() =>
            this.Argument.GetHashCode();
    }

    public abstract class UnaryOperatorTerm<T> : UnaryOperatorTerm
        where T : UnaryOperatorTerm
    {
        protected UnaryOperatorTerm(Term argument) :
            base(argument)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T term ?
                this.Argument.Equals(term.Argument) :
                false;
    }

    public abstract class LogicalUnaryOperatorTerm : UnaryOperatorTerm
    {
        internal LogicalUnaryOperatorTerm(Term argument) :
            base(argument)
        { }

        public override sealed Term HigherOrder =>
            BooleanTerm.Type;

        protected override sealed Term Infer(InferContext context, Term argument)
        {
            context.Unify(argument.HigherOrder, BooleanTerm.Type);

            return this.Create(argument);
        }
    }

    public abstract class LogicalUnaryOperatorTerm<T> : LogicalUnaryOperatorTerm
        where T : LogicalUnaryOperatorTerm
    {
        protected LogicalUnaryOperatorTerm(Term argument) :
            base(argument)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T term ?
                this.Argument.Equals(term.Argument) :
                false;
    }
}
