using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class StringExpression : ConstantExpression<string>
    {
        internal StringExpression(string value) :
            base(value)
        { }

        protected override string FormatReadableString(FormatContext context) =>
            $"\"{this.Value}\"";
    }
}
