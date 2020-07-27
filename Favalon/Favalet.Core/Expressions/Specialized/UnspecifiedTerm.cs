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
            PlaceholderTerm.Create(context);

        protected override IExpression Fixup(IReduceContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    "?" :
                    "Unspecified");

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
        public static readonly FunctionExpression Function =
            FunctionExpression.Create(Instance, Instance);
    }
}
