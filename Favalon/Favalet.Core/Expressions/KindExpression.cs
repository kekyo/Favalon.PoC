using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class KindExpression : PseudoExpression
    {
        private KindExpression()
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Kind)" : "*";

        internal static readonly KindExpression Instance = new KindExpression();
    }
}
