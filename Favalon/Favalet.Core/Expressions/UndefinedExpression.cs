using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class UndefinedExpression : Expression
    {
        private UndefinedExpression() :
            base(null!)
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            "?";

        public static readonly UndefinedExpression Instance = new UndefinedExpression();
    }
}
