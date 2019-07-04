using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class LambdaExpression : Expression
    {
        public LambdaExpression(Expression parameter, Expression expression, Expression higherOrder) :
            base(higherOrder)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        public readonly Expression Parameter;
        public readonly Expression Expression;

        protected override FormattedString FormatReadableString(FormatContext context)
        {
            var arrow = (context.FormatNaming == FormatNamings.Fancy) ? "→" : "->";
            return FormattedString.RequiredEnclosing(
                $"{FormatReadableString(context, this.Parameter, true)} {arrow} {FormatReadableString(context, this.Expression, false)}");
        }

        protected override Expression VisitInferring(Environment environment, Expression higherOrderHint) =>
            throw new NotImplementedException();

        protected override Expression VisitResolving(Environment environment)
        {
            var parameter = VisitResolving(environment, this.Parameter);
            var expression = VisitResolving(environment, this.Expression);
            var higherOrder = VisitResolving(environment, this.HigherOrder);

            return new LambdaExpression(parameter, expression, higherOrder);
        }
    }
}
