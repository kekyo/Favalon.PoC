﻿using Favalet.Contexts;
using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class PlaceholderTerm :
        Expression, ITerm
    {
        private readonly IReduceContext context;
        private readonly Lazy<PlaceholderTerm> higherOrder;

        public readonly int Index;

        private PlaceholderTerm(IReduceContext context, int index)
        {
            this.context = context;
            this.Index = index;
            this.higherOrder = new Lazy<PlaceholderTerm>(
                () => new PlaceholderTerm(this.context, this.context.NewPlaceholderIndex()));
        }

        public IExpression HigherOrder =>
            this.higherOrder.Value;

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(PlaceholderTerm rhs) =>
            this.Index == rhs.Index;

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is PlaceholderTerm rhs && this.Equals(rhs);

        public IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new PlaceholderTerm(this.context, this.Index);
            }
        }

        public IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.FixupHigherOrder(this.HigherOrder);

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new PlaceholderTerm(this.context, this.Index);
            }
        }

        public IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    $"'{this.Index}" :
                    $"Placeholder '{this.Index}");

        internal static PlaceholderTerm Create(IReduceContext context) =>
            new PlaceholderTerm(context, context.NewPlaceholderIndex());
    }
}
