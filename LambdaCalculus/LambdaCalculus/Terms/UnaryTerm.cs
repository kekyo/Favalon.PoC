using Favalon.Contexts;

namespace Favalon.Terms
{
    public abstract class UnaryTerm : Term
    {
        public readonly Term Argument;

        internal UnaryTerm(Term argument) =>
            this.Argument = argument;

        protected abstract Term Create(Term argument, Term higherOrder);

        protected virtual Term Infer(InferContext context, Term argument, Term higherOrderHint) =>
            this.Create(argument, higherOrderHint);

        public override sealed Term Infer(InferContext context, Term higherOrderHint)
        {
            var higherOrder = this.HigherOrder.Infer(context, higherOrderHint.HigherOrder);
            higherOrder = context.Unify(higherOrder, higherOrderHint).Term;

            var argument = this.Argument.Infer(context, higherOrder);
            context.Unify(argument.HigherOrder, higherOrder);            

            return this.Infer(context, argument, higherOrder);
        }

        protected virtual Term Fixup(FixupContext context, Term argument, Term higherOrder) =>
            this.Create(argument, higherOrder);

        public override sealed Term Fixup(FixupContext context) =>
            this.Fixup(context, this.Argument.Fixup(context), this.HigherOrder.Fixup(context));

        public override int GetHashCode() =>
            this.Argument.GetHashCode();
    }

    public abstract class UnaryTerm<T> : UnaryTerm
        where T : UnaryTerm
    {
        protected UnaryTerm(Term argument) :
            base(argument)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T term ?
                this.Argument.Equals(term.Argument) :
                false;
    }
}
