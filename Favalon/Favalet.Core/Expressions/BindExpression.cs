using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class BindExpression : Expression
    {
        internal BindExpression(BoundVariableExpression bound, Expression expression, bool recursiveBind, Expression higherOrder) :
            base(higherOrder)
        {
            this.Bound = bound;
            this.Expression = expression;
            this.RecursiveBind = recursiveBind;
        }

        public new readonly BoundVariableExpression Bound;
        public readonly Expression Expression;
        public new readonly bool RecursiveBind;

        protected override FormattedString FormatReadableString(FormatContext context)
        {
            var rec = this.RecursiveBind ? "rec " : string.Empty;
            return FormattedString.RequiredEnclosing(
                $"{rec}{FormatReadableString(context, this.Bound, true)} = {FormatReadableString(context, this.Expression, context.FormatNaming != FormatNamings.Friendly)}");
        }

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint)
        {
            var higherOrder = environment.Unify(higherOrderHint, this.HigherOrder);

            if (this.RecursiveBind)
            {
                var bound = environment.Visit(this.Bound, higherOrder);
                var expression = environment.Visit(this.Expression, bound.HigherOrder);

                return new BindExpression(bound, expression, true, expression.HigherOrder);
            }
            else
            {
                var expression = environment.Visit(this.Expression, higherOrder);
                var bound = environment.Visit(this.Bound, expression.HigherOrder);

                return new BindExpression(bound, expression, false, bound.HigherOrder);
            }
        }

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var bound = environment.Visit(this.Bound);
            var expression = environment.Visit(this.Expression);
            var higherOrder = environment.Visit(this.HigherOrder);

            return new BindExpression(bound, expression, this.RecursiveBind, higherOrder);
        }
    }
}
