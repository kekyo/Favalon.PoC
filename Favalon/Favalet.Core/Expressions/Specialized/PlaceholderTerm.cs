using Favalet.Contexts;
using System;
using System.Diagnostics;

namespace Favalet.Expressions.Specialized
{
    public sealed class PlaceholderTerm :
        Expression, ITerm
    {
        private readonly IReduceContext context;
        private PlaceholderTerm? higherOrder;

        public readonly int Index;

        private PlaceholderTerm(IReduceContext context, int index)
        {
            this.context = context;
            this.Index = index;
        }

        public override IExpression HigherOrder
        {
            [DebuggerStepThrough]
            get
            {
                if (this.higherOrder == null)
                {
                    lock (this)
                    {
                        if (this.higherOrder == null)
                        {
                            this.higherOrder =
                                new PlaceholderTerm(this.context, this.context.NewPlaceholderIndex());
                        }
                    }
                }
                return this.higherOrder;
            }
        }

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(PlaceholderTerm rhs) =>
            this.Index == rhs.Index;

        public override bool Equals(IExpression? other) =>
            other is PlaceholderTerm rhs && this.Equals(rhs);

        protected override IExpression Infer(IReduceContext context)
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

        protected override IExpression Fixup(IReduceContext context)
        {
            if (context.ResolvePlaceholderIndex(this.Index) is IExpression resolved)
            {
                return resolved;
            }
            else
            {
                return this;
            }
        }

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    $"'{this.Index}" :
                    $"Placeholder '{this.Index}");

        [DebuggerStepThrough]
        internal static PlaceholderTerm Create(IReduceContext context) =>
            new PlaceholderTerm(context, context.NewPlaceholderIndex());
    }
}
