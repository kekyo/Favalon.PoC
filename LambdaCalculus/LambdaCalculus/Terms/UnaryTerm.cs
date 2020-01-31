using Favalon.Terms.Contexts;

namespace Favalon.Terms
{
    public abstract class UnaryTerm : Term
    {
        public readonly Term Argument;

        protected UnaryTerm(Term argument) =>
            this.Argument = argument;

        protected abstract Term OnCreate(Term argument, Term higherOrder);

        public override Term Infer(InferContext context)
        {
            var argument = this.Argument.Infer(context);
            var higherOrder = context.ResolveHigherOrder(this.HigherOrder);

            context.Unify(argument.HigherOrder, higherOrder);

            return
                this.Argument.EqualsWithHigherOrder(argument) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    this.OnCreate(argument, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var argument = this.Argument.Fixup(context);
            var higherOrder = this.HigherOrder.Fixup(context);

            return
                this.Argument.EqualsWithHigherOrder(argument) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    this.OnCreate(argument, higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            var argument = this.Argument.Reduce(context);
            var higherOrder = this.HigherOrder.Reduce(context);

            return
                this.Argument.EqualsWithHigherOrder(argument) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
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
        protected UnaryTerm(Term argument) :
            base(argument)
        { }

        protected override sealed bool OnEquals(EqualsContext context, Term? other) =>
            other is T term ?
               this.Argument.Equals(context, term.Argument) :
               false;
    }
}
