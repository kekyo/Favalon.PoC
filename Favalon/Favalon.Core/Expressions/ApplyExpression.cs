using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class ApplyExpression : Expression
    {
        public ApplyExpression(Expression function, Expression parameter, Expression higherOrder) :
            base(higherOrder)
        {
            this.Function = function;
            this.Parameter = parameter;
        }

        public readonly Expression Function;
        public readonly Expression Parameter;

        protected override string FormatReadableString() =>
            $"{this.Function.ReadableString} {this.Parameter.ReadableString}";
    }
}
