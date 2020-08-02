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
        IIdentityTerm CreatePlaceholder(
            PlaceholderOrderHints orderHint = PlaceholderOrderHints.TypeOrAbove);
    }

    public sealed class PlaceholderTerm :
        Expression, IIdentityTerm
    {
        private IPlaceholderProvider provider;
        private IIdentityTerm? higherOrder;

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
        }

        public override IExpression HigherOrder
        {
            [DebuggerStepThrough]
            get
            {
                if (this.OrderHint >= PlaceholderOrderHints.Fourth)
                {
                    return TerminationTerm.Instance;
                }

                if (this.higherOrder == null)
                {
                    lock (this)
                    {
                        if (this.higherOrder == null)
                        {
                            this.higherOrder = this.provider.CreatePlaceholder(
                                this.OrderHint + 1);
                        }
                    }
                }
                return this.higherOrder;
            }
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

    public static class PlaceholderTermExtension
    {
        [DebuggerStepThrough]
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
