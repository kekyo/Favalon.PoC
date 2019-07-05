using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class BindExpression : Expression
    {
        internal BindExpression(BoundVariableExpression bound, Expression expression, Expression higherOrder) :
            base(higherOrder)
        {
            this.Bound = bound;
            this.Expression = expression;
        }

        public readonly BoundVariableExpression Bound;
        public readonly Expression Expression;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            FormattedString.RequiredEnclosing(
                $"{FormatReadableString(context, this.Bound, true)} = {FormatReadableString(context, this.Expression, context.FormatNaming != FormatNamings.Friendly)}");

        protected override Expression VisitInferring(Environment environment, Expression higherOrderHint)
        {
            var higherOrder = Unify(environment, higherOrderHint, this.HigherOrder);

            var bound = VisitInferring(environment, this.Bound, higherOrder);
            var expression = VisitInferring(environment, this.Expression, bound.HigherOrder);

            return new BindExpression(bound, expression, expression.HigherOrder);
        }

        protected override Expression VisitResolving(Environment environment)
        {
            var bound = VisitResolving(environment, this.Bound);
            var expression = VisitResolving(environment, this.Expression);
            var higherOrder = VisitResolving(environment, this.HigherOrder);

            return new BindExpression(bound, expression, higherOrder);
        }
    }
}
