using System;

#pragma warning disable 659

namespace Favalon
{
    public abstract partial class Term : IEquatable<Term?>
    {
        protected Term()
        { }

        public abstract Term HigherOrder { get; }

        public abstract Term Infer(InferContext context);

        public abstract Term Fixup(FixupContext context);

        public abstract Term Reduce(ReduceContext context);

        public abstract bool Equals(Term? other);

        bool IEquatable<Term?>.Equals(Term? other) =>
            this.Equals(other);

        public override sealed bool Equals(object? other) =>
            this.Equals(other as Term);

        public void Deconstruct(out Term higherOrder) =>
            higherOrder = this.HigherOrder;
    }

    ////////////////////////////////////////////////////////////

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
            context.LookupUnifiedTerm(this);

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
    }

    public sealed class ConstantTerm : HigherOrderLazyTerm
    {
        private static readonly int hashCode =
            typeof(ConstantTerm).GetHashCode();

        public readonly object Value;

        internal ConstantTerm(object value) =>
            this.Value = value;

        protected override Term GetHigherOrder() =>
            Type(this.Value.GetType());

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is ConstantTerm rhs ? this.Value.Equals(rhs.Value) : false;

        public override int GetHashCode() =>
            hashCode ^ this.Value.GetHashCode();
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

        public static readonly KindTerm Instance =
            new KindTerm();
    }
}
