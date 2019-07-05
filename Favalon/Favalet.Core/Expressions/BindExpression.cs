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

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint)
        {
            var higherOrder = environment.Unify(higherOrderHint, this.HigherOrder);

            var bound = environment.Visit(this.Bound, higherOrder);
            var expression = environment.Visit(this.Expression, bound.HigherOrder);
            var expressionHigherOrder = environment.Unify(bound.HigherOrder, expression.HigherOrder);

            return new BindExpression(bound, expression, expressionHigherOrder);
        }

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var bound = environment.Visit(this.Bound);
            var expression = environment.Visit(this.Expression);
            var higherOrder = environment.Visit(this.HigherOrder);

            return new BindExpression(bound, expression, higherOrder);
        }
    }
}
