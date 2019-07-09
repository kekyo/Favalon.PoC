using Favalet.Expressions.Internals;
using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class KindExpression : PseudoExpression
    {
        private KindExpression() :
            base(null!)
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Kind)" : "*";

        internal static readonly KindExpression Instance =
            new KindExpression();
    }
}
