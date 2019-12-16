using Favalon.Contexts;

namespace Favalon.Terms
{
    public abstract class UnaryTerm : Term
    {
        public readonly Term Argument;

        internal UnaryTerm(Term argument) =>
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
