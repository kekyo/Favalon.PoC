using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class KindExpression : Expression
    {
        private KindExpression() :
            base(null!)
        { }

        protected override string FormatReadableString() =>
            "*";

        public static readonly KindExpression Instance = new KindExpression();
    }
}
