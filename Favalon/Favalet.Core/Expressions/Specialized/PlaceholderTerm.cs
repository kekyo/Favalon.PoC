using Favalet.Contexts;
using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class PlaceholderTerm :
        Expression, ITerm
    {
        public readonly int Index;

        private PlaceholderTerm(int index, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Index = index;
        }

        public IExpression HigherOrder { get; }

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(PlaceholderTerm rhs) =>
            this.Index == rhs.Index;

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is PlaceholderTerm rhs && this.Equals(rhs);

        public IExpression Infer(IReduceContext context) =>
            this;

        public IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    $"'{this.Index}" :
                    $"Placeholder '{this.Index}");

        internal static PlaceholderTerm Create(
            int index, IExpression higherOrder) =>
            new PlaceholderTerm(index, higherOrder);
    }
}
