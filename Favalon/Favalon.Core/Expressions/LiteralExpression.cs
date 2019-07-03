using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class LiteralExpression : Expression
    {
        public LiteralExpression(object value) :
            base(new VariableExpression(value.GetType().FullName, KindExpression.Instance)) =>
        this.Value = value;

        public readonly object Value;

        protected override string FormatReadableString() =>
            this.Value.ToString();
    }
}
