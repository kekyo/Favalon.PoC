using Favalon.Terms.Contexts;

namespace Favalon.Terms.Algebraic
{
    public sealed class EmptyTerm : Term
    {
        private static readonly int hashCode =
           typeof(EmptyTerm).GetHashCode();

        private EmptyTerm(Term higherOrder) =>
            this.HigherOrder = higherOrder;

        public override Term HigherOrder { get; }

        internal override bool ValidTerm =>
            false;

        public override Term Infer(InferContext context)
        {
            var higherOrder = context.ResolveHigherOrder(this.HigherOrder);

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new EmptyTerm(higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var higherOrder = this.HigherOrder.Fixup(context);

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new EmptyTerm(higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new EmptyTerm(higherOrder);
        }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is EmptyTerm;

        public override int GetHashCode() =>
            hashCode;

        protected override bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            false;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "{}";

        public static readonly EmptyTerm Instance =
            new EmptyTerm(UnspecifiedTerm.Instance);

        public static EmptyTerm Create(Term higherOrder) =>
            new EmptyTerm(higherOrder);
    }
}
