using Favalet.Expressions.Internals;
using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class UnspecifiedExpression : Expression
    {
        private UnspecifiedExpression() :
            base(null!)
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Unspecified)" : "_";

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            throw new NotImplementedException();

        protected override Expression VisitResolving(IResolvingEnvironment environment) =>
            this;

        internal static readonly UnspecifiedExpression Instance =
            new UnspecifiedExpression();
    }
}
