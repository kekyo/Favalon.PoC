using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class LiteralExpression : ConstantExpression
    {
        public new readonly object Value;

        internal LiteralExpression(object value) :
            base(new TypeExpression(value.GetType().FullName)) =>
            this.Value = value;

        internal override object GetValue() =>
            this.Value;

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context) =>
            (this.Value.ToString(), !this.Value.GetType().IsPrimitive);
    }
}
