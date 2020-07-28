using Favalet.Contexts;
using System.Diagnostics;

namespace Favalet.Expressions.Specialized
{
    public interface IBoundSymbolTerm :
        ITerm
    {
        string Symbol { get; }
    }

    public sealed class BoundSymbolTerm :
        Expression, IBoundSymbolTerm
    {
        public readonly string Symbol;

        private BoundSymbolTerm(string symbol, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Symbol = symbol;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string IBoundSymbolTerm.Symbol =>
            this.Symbol;

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(IBoundSymbolTerm rhs) =>
            this.Symbol.Equals(rhs.Symbol);

        public override bool Equals(IExpression? other) =>
            other is IBoundSymbolTerm rhs && this.Equals(rhs);

        protected override IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new BoundSymbolTerm(this.Symbol, higherOrder);
            }
        }

        protected override IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.Fixup(this.HigherOrder);

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new BoundSymbolTerm(this.Symbol, higherOrder);
            }
        }

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                context.IsSimple ?
                    this.Symbol :
                    $"BoundSymbol {this.Symbol}");

        [DebuggerStepThrough]
        public static BoundSymbolTerm Create(string symbol, IExpression higherOrder) =>
            new BoundSymbolTerm(symbol, higherOrder);
        [DebuggerStepThrough]
        public static BoundSymbolTerm Create(string symbol) =>
            new BoundSymbolTerm(symbol, UnspecifiedTerm.Instance);
    }
}
