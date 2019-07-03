using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class UndefinedExpression : Expression
    {
        private UndefinedExpression() :
            base(null!)
        { }

        protected override string FormatReadableString() =>
            "?";

        public static readonly UndefinedExpression Instance = new UndefinedExpression();
    }
}
