using Favalet.Contexts;
using System.Collections;
using System.Diagnostics;
using System.Xml.Linq;

namespace Favalet.Expressions.Specialized
{
    public enum PlaceholderOrderHints
    {
        VariableOrAbove = 0,
        TypeOrAbove,
        KindOrAbove,
        Fourth
    }

    public interface IPlaceholderProvider
    {
        IPlaceholderTerm CreatePlaceholder(
            PlaceholderOrderHints orderHint = PlaceholderOrderHints.TypeOrAbove);
    }

    public interface IPlaceholderTerm :
        ITerm
    {
        int Index { get; }
    }

    public sealed class PlaceholderTerm :
        Expression, IPlaceholderTerm
    {
        private readonly PlaceholderOrderHints candidateOrder;
        private IPlaceholderProvider provider;
        private IPlaceholderTerm? higherOrder;

        public readonly int Index;

        [DebuggerStepThrough]
        private PlaceholderTerm(
            IPlaceholderProvider provider,
            int index,
            PlaceholderOrderHints candidateOrder)
        {
            this.candidateOrder = candidateOrder;
            this.provider = provider;
            this.Index = index;
        }

        public override IExpression HigherOrder
        {
            [DebuggerStepThrough]
            get
            {
                // Helps for inferring:
                //   Nested higher order has ceil limit to 4th order.
                if (this.candidateOrder >= PlaceholderOrderHints.KindOrAbove)
                {
                    return FourthTerm.Instance;
                }

                if (this.higherOrder == null)
                {
                    lock (this)
                    {
                        if (this.higherOrder == null)
                        {
                            this.higherOrder = this.provider.CreatePlaceholder(
                                this.candidateOrder + 1);
                        }
                    }
                }
                return this.higherOrder;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int IPlaceholderTerm.Index
        {
            [DebuggerStepThrough]
            get => this.Index;
        }

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(IPlaceholderTerm rhs) =>
            this.Index == rhs.Index;

        public override bool Equals(IExpression? other) =>
            other is IPlaceholderTerm rhs && this.Equals(rhs);

        protected override IExpression Infer(IReduceContext context) =>
            this;

        protected override IExpression Fixup(IReduceContext context)
        {
            if (context.ResolvePlaceholderIndex(this.Index) is IExpression resolved)
            {
                return context.Fixup(resolved);
            }
            else
            {
                return this;
            }
        }

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            new[] { new XAttribute("index", this.Index) };

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                $"'{this.Index}");

        [DebuggerStepThrough]
        internal static PlaceholderTerm Create(
            IPlaceholderProvider provider,
            int index,
            PlaceholderOrderHints orderHint) =>
            new PlaceholderTerm(
                provider,
                index,
                orderHint);
    }

    public static class PlaceholderTermExtension
    {
        [DebuggerStepThrough]
        public static void Deconstruct(
            this IPlaceholderTerm placeholder,
            out int index) =>
            index = placeholder.Index;
    }
}
