using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class LambdaExpression : ValueExpression
    {
        internal LambdaExpression(Expression parameter, Expression expression, Expression higherOrder) :
            base(higherOrder)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        public readonly Expression Parameter;
        public readonly Expression Expression;

        protected override FormattedString FormatReadableString(FormatContext context)
        {
            var arrow = (context.FormatOperator == FormatOperators.Fancy) ? "→" : "->";
            return FormattedString.RequiredEnclosing(
                $"{FormatReadableString(context, this.Parameter, true)} {arrow} {FormatReadableString(context, this.Expression, context.FormatNaming != FormatNamings.Friendly)}");
        }

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint)
        {
            var higherOrder = environment.Unify(higherOrderHint, this.HigherOrder);
            var lambdaHigherOrder = higherOrder as LambdaExpression;

            var parameter = environment.Visit(this.Parameter, lambdaHigherOrder?.Parameter ?? UnspecifiedExpression.Instance);
            var expression = environment.Visit(this.Expression, lambdaHigherOrder?.Expression ?? UnspecifiedExpression.Instance);

            var newLambdaHigherOrder = environment.Unify(higherOrder,
                new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, UnspecifiedExpression.Instance));

            return new LambdaExpression(parameter, expression, newLambdaHigherOrder);
        }

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var parameter = environment.Visit(this.Parameter);
            var expression = environment.Visit(this.Expression);
            var higherOrder = environment.Visit(this.HigherOrder);

            return new LambdaExpression(parameter, expression, higherOrder);
        }
    }
}
