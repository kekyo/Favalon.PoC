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
        private readonly PlaceholderOrderHints candidateOrder;
        private IPlaceholderProvider provider;
        private IIdentityTerm? higherOrder;

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
                        this.provider, this.Index, this.candidateOrder);
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
}
