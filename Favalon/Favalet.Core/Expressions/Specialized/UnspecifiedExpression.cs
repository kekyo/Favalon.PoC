using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class UnspecifiedExpression : Expression
    {
        private UnspecifiedExpression(Expression higherOrder) :
            base(higherOrder)
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Unspecified)" : "_";

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            throw new NotImplementedException();

        protected override Expression VisitResolving(IResolvingEnvironment environment) =>
            this;

        internal static readonly UnspecifiedExpression Instance =
            new UnspecifiedExpression(null!);
        internal static new readonly UnspecifiedExpression Kind =
            new UnspecifiedExpression(KindExpression.Instance);
        internal static new readonly UnspecifiedExpression Type =
            new UnspecifiedExpression(TypeExpression.Instance);
    }
}
