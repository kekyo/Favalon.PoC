using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class BoolExpression : ConstantExpression<bool>
    {
        internal BoolExpression(bool value) :
            base(value)
        {
        }

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context) =>
            (this.Value.ToString(), false);
    }
}
