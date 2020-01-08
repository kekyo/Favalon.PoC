using Favalon.Contexts;
//using Favalon.Terms.Types;
using LambdaCalculus.Contexts;
using System;

#pragma warning disable 659

namespace Favalon.Terms
{
    public abstract class HigherOrderHoldTerm : Term
    {
        protected HigherOrderHoldTerm(Term higherOrder) =>
            this.HigherOrder = higherOrder;

        public override sealed Term HigherOrder { get; }
    }

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

    public sealed class UnspecifiedTerm : Term
    {
        private static readonly int hashCode =
           typeof(UnspecifiedTerm).GetHashCode();

        private UnspecifiedTerm()
        { }

        public override Term HigherOrder =>
            null!;

        public override Term Infer(InferContext context) =>
            context.CreatePlaceholder(Instance);

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is UnspecifiedTerm;

        public override int GetHashCode() =>
            hashCode;

        protected override bool IsInclude(HigherOrderDetails higherOrderDetail) =>
            false;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "?";

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }

    public sealed class PlaceholderTerm : HigherOrderHoldTerm
    {
        private static readonly int hashCode =
            typeof(PlaceholderTerm).GetHashCode();

        public readonly int Index;

        internal PlaceholderTerm(int index, Term higherOrder) :
            base(higherOrder) =>
            this.Index = index;

        public override Term Infer(InferContext context) =>
            new PlaceholderTerm(this.Index, this.HigherOrder.Infer(context));

        public override Term Fixup(FixupContext context) =>
            context.LookupUnifiedTerm(this) is Term term ?
                term.Fixup(context) :
                this;

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);
            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new PlaceholderTerm(this.Index, higherOrder);
        }

        public override bool Equals(Term? other) =>
            other is PlaceholderTerm rhs ? this.Index.Equals(rhs.Index) : false;

        public override int GetHashCode() =>
            hashCode ^ this.Index;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"'{this.Index}";
    }

    public sealed class KindTerm : Term
    {
        private static readonly int hashCode =
           typeof(KindTerm).GetHashCode();

        private KindTerm()
        { }

        public override Term HigherOrder =>
            null!;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is KindTerm;

        public override int GetHashCode() =>
            hashCode;

        protected override bool IsInclude(HigherOrderDetails higherOrderDetail) =>
            false;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "*";

        public static readonly KindTerm Instance =
            new KindTerm();
    }
}
