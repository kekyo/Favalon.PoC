using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Internals
{
    public sealed class UnspecifiedExpression : PseudoExpression
    {
        private UnspecifiedExpression()
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Unspecified)" : "?";

        public static readonly UnspecifiedExpression Instance = new UnspecifiedExpression();
    }
}
