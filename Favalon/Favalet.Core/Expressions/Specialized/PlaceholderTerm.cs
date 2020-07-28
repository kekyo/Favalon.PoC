﻿using Favalet.Contexts;
using System;
using System.Diagnostics;

namespace Favalet.Expressions.Specialized
{
    public enum PlaceholderOrders
    {
        Variable = 0,
        Type,
        Kind,
        Fourth
    }

    public interface IPlaceholderProvider
    {
        int AssignPlaceholderIndex();
    }

    public interface IPlacehoderTerm :
        ITerm
    {
        int Index { get; }
    }

    public sealed class PlaceholderTerm :
        Expression, IPlacehoderTerm
    {
        private readonly PlaceholderOrders candidateOrder;
        private IPlaceholderProvider provider;
        private PlaceholderTerm? higherOrder;

        public readonly int Index;

        private PlaceholderTerm(
            IPlaceholderProvider provider,
            int index,
            PlaceholderOrders candidateOrder)
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
                if (this.candidateOrder >= PlaceholderOrders.Kind)
                {
                    return FourthTerm.Instance;
                }

                if (this.higherOrder == null)
                {
                    lock (this)
                    {
                        if (this.higherOrder == null)
                        {
                            this.higherOrder = new PlaceholderTerm(
                                this.provider, 
                                this.provider.AssignPlaceholderIndex(),
                                this.candidateOrder + 1);
                        }
                    }
                }
                return this.higherOrder;
            }
        }

        int IPlacehoderTerm.Index =>
            this.Index;

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(IPlacehoderTerm rhs) =>
            this.Index == rhs.Index;

        public override bool Equals(IExpression? other) =>
            other is IPlacehoderTerm rhs && this.Equals(rhs);

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

        public override string GetPrettyString(PrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                context.IsSimple ?
                    $"'{this.Index}" :
                    $"Placeholder '{this.Index}");

        [DebuggerStepThrough]
        internal static PlaceholderTerm Create(
            IPlaceholderProvider provider,
            int index,
            PlaceholderOrders candidateOrder) =>
            new PlaceholderTerm(
                provider,
                index,
                candidateOrder);
    }
}
