using Favalon.Terms.Contexts;

#pragma warning disable 659

namespace Favalon.Terms
{
    public abstract class HigherOrderLazyTerm : Term
    {
        private Term? higherOrder;

        protected HigherOrderLazyTerm()
        { }

        public override sealed Term HigherOrder
        {
            get
            {
                if (higherOrder == null)
                {
                    lock (this)
                    {
                        if (higherOrder == null)
                        {
                            higherOrder = this.GetHigherOrder();
                        }
                    }
                }
                return higherOrder;
            }
        }

        protected abstract Term GetHigherOrder();
    }

    ////////////////////////////////////////////////////////////

    internal sealed class TerminationTerm : Term
    {
        private static readonly int hashCode =
           typeof(TerminationTerm).GetHashCode();

        private TerminationTerm()
        { }

        public override Term HigherOrder =>
            Instance;

        internal override bool ValidTerm =>
            false;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is TerminationTerm;

        public override int GetHashCode() =>
            hashCode;

        protected override bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            false;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "#";

        public static readonly TerminationTerm Instance =
            new TerminationTerm();
    }

    public sealed class UnspecifiedTerm : Term
    {
        private static readonly int hashCode =
           typeof(UnspecifiedTerm).GetHashCode();

        private UnspecifiedTerm()
        { }

        public override Term HigherOrder =>
            TerminationTerm.Instance;

        public override Term Infer(InferContext context) =>
            context.CreatePlaceholder(Instance);

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is UnspecifiedTerm;

        public override int GetHashCode() =>
            hashCode;

        protected override bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            false;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "_";

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }

    public sealed class PlaceholderTerm : Term
    {
        private static readonly int hashCode =
            typeof(PlaceholderTerm).GetHashCode();

        public readonly int Index;

        internal PlaceholderTerm(int index, Term higherOrder)
        {
            this.Index = index;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public override Term Infer(InferContext context)
        {
            // Will stop infinite inferring.
            if (this.HigherOrder is UnspecifiedTerm)
            {
                return this;
            }

            var higherOrder = context.ResolveHigherOrder(this.HigherOrder);

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new PlaceholderTerm(this.Index, higherOrder);
        }

        public override Term Fixup(FixupContext context) =>
            context.LookupUnifiedTerm(this) is Term term ?
                term.Fixup(context) :
                this;

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);

            return 
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new PlaceholderTerm(this.Index, higherOrder);
        }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is PlaceholderTerm rhs ? (this.Index == rhs.Index) : false;

        public override int GetHashCode() =>
            hashCode ^ this.Index;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"'{this.Index}";
    }
}
