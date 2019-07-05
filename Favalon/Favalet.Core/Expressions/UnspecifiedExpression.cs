using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class UnspecifiedExpression : PseudoExpression
    {
        private UnspecifiedExpression()
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            "?";

        public static readonly UnspecifiedExpression Instance = new UnspecifiedExpression();
    }
}
