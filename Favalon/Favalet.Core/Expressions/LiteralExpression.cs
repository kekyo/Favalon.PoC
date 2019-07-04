using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class LiteralExpression : Expression
    {
        public LiteralExpression(object value) :
            base(new VariableExpression(value.GetType().FullName, KindExpression.Instance)) =>
        this.Value = value;

        public readonly object Value;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (this.Value is string) ? $"\"{this.Value}\"" : this.Value.ToString();
    }
}
