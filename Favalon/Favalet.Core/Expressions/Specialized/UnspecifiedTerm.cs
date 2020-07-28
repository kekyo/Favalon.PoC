using Favalet.Contexts;
using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class UnspecifiedTerm :
        Expression, ITerm
    {
        private UnspecifiedTerm()
        { }

        public override IExpression HigherOrder =>
            null!;

        public bool Equals(UnspecifiedTerm rhs) =>
            rhs != null;

        public override bool Equals(IExpression? other) =>
            other is UnspecifiedTerm;

        protected override IExpression Infer(IReduceContext context) =>
            context is IPlaceholderProvider provider ?
                (IExpression)PlaceholderTerm.Create(
                    provider,
                    provider.AssignPlaceholderIndex(),
                    PlaceholderOrders.Type) :
                this;

        protected override IExpression Fixup(IReduceContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                context.IsSimple ?
                    "_" :
                    "Unspecified");

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }
}
