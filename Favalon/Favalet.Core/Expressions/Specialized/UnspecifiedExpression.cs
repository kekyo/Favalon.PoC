using Favalet.Expressions.Internals;
using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class UnspecifiedExpression : PseudoExpression
    {
        private UnspecifiedExpression() :
            base(null!)
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Unspecified)" : "?";

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            throw new NotImplementedException();

        internal static readonly UnspecifiedExpression Instance = new UnspecifiedExpression();
    }
}
