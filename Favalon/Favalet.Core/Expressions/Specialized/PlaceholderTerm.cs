using Favalet.Contexts;
using Favalet.Internal;
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
        IIdentityTerm CreatePlaceholder(PlaceholderOrderHints orderHint);
    }

    public sealed class PlaceholderTerm :
        Expression, IIdentityTerm
    {
        private IPlaceholderProvider provider;
        private readonly LazySlim<IExpression> higherOrder;

        public readonly int Index;
        public readonly PlaceholderOrderHints OrderHint;

        [DebuggerStepThrough]
        private PlaceholderTerm(
            IPlaceholderProvider provider,
            int index,
            PlaceholderOrderHints orderHint)
        {
            Debug.Assert(orderHint <= PlaceholderOrderHints.Fourth);

            this.provider = provider;
            this.Index = index;
            this.OrderHint = orderHint;
            this.higherOrder = LazySlim.Create(() =>
                (this.OrderHint >= PlaceholderOrderHints.Fourth) ?
                    (IExpression)DeadEndTerm.Instance :
                    this.provider.CreatePlaceholder(this.OrderHint + 1));
        }

        public override IExpression HigherOrder
        {
            [DebuggerStepThrough]
            get => this.higherOrder.Value;
        }

        public string Symbol
        {
            [DebuggerStepThrough]
            get => $"'{this.Index}";
        }

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(IIdentityTerm rhs) =>
            this.Symbol.Equals(rhs.Symbol);

        public override bool Equals(IExpression? other) =>
            other is IIdentityTerm rhs && this.Equals(rhs);

        protected override IExpression Infer(IReduceContext context) =>
            this;

        protected override IExpression Fixup(IReduceContext context)
        {
            if (context.Resolve(this.Symbol) is IExpression resolved)
            {
                return context.Fixup(resolved);
            }
            else
            {
                var higherOrder = context.Fixup(this.HigherOrder);

                if (object.ReferenceEquals(this.HigherOrder, higherOrder))
                {
                    return this;
                }
                else
                {
                    return new PlaceholderTerm(
                        this.provider, this.Index, this.OrderHint);
                }
            }
        }

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            new[] { new XAttribute("index", this.Index) };

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                this.Symbol);

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

    [DebuggerStepThrough]
    public static class PlaceholderTermExtension
    {
        public static void Deconstruct(
            this PlaceholderTerm placeholder,
            out string symbol,
            out PlaceholderOrderHints orderHint)
        {
            symbol = placeholder.Symbol;
            orderHint = placeholder.OrderHint;
        }
    }
}
