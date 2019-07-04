using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class KindExpression : Expression
    {
        private KindExpression() :
            base(null!)
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            "*";

        public static readonly KindExpression Instance = new KindExpression();
    }
}
