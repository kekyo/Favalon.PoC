using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
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

        protected override FormattedString FormatReadableString(FormatContext context) =>
            FormattedString.RequiredEnclose(
                (this.Parameter is ApplyExpression) ?
                    $"{FormatReadableString(context, this.Function, true)} ({FormatReadableString(context, this.Parameter, false)})" :
                    $"{FormatReadableString(context, this.Function, true)} {FormatReadableString(context, this.Parameter, true)}");
    }
}
