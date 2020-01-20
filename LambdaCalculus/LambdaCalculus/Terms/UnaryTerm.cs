using Favalon.Contexts;

namespace Favalon.Terms
{
    public abstract class UnaryTerm : Term
    {
        public readonly Term Argument;

        internal UnaryTerm(Term argument, Term higherOrder)
        {
            this.Argument = argument;
            this.HigherOrder = higherOrder;
        }

        public override sealed Term HigherOrder { get; }

        protected abstract Term OnCreate(Term argument, Term higherOrder);

        public override Term Infer(InferContext context)
        {
            var argument = this.Argument.Infer(context);
            var higherOrder = this.HigherOrder.Infer(context);

            context.Unify(argument.HigherOrder, higherOrder);

            return
                object.ReferenceEquals(argument, this.Argument) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    this.OnCreate(argument, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var argument = this.Argument.Fixup(context);
            var higherOrder = this.HigherOrder.Fixup(context);

            return
                object.ReferenceEquals(argument, this.Argument) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    this.OnCreate(argument, higherOrder);
        }

        public void Deconstruct(out Term argument, out Term higherOrder)
        {
            argument = this.Argument;
            higherOrder = this.HigherOrder;
        }
    }

    public abstract class UnaryTerm<T> : UnaryTerm
        where T : UnaryTerm
    {
        protected UnaryTerm(Term argument, Term higherOrder) :
            base(argument, higherOrder)
        { }

        protected override sealed bool OnEquals(Term? other) =>
            other is T term ?
               this.Argument.Equals(term.Argument) :
               false;
    }
}
