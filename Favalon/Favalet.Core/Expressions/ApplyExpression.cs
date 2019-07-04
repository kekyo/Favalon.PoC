using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class ApplyExpression : Expression
    {
        public ApplyExpression(Expression function, Expression argument, Expression higherOrder) :
            base(higherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public readonly Expression Function;
        public readonly Expression Argument;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            FormattedString.RequiredEnclosing(
                $"{FormatReadableString(context, this.Function, true)} {FormatReadableString(context, this.Argument, true)}");

        protected override Expression VisitInferring(Environment environment, Expression higherOrderHint)
        {
            var higherOrder = CreatePlaceholderIfRequired(environment, higherOrderHint);
            var argument = VisitInferring(environment, this.Argument, UndefinedExpression.Instance);
            var function = VisitInferring(environment, this.Function,
                new LambdaExpression(argument.HigherOrder, higherOrder, UndefinedExpression.Instance));

            // Apply3: Update()
            if (function.HigherOrder is LambdaExpression functionHigherOrder)
            {
                var updated = Unify(environment, argument.HigherOrder, functionHigherOrder.Parameter);
                UpdateHigherOrder(argument, updated);
            }

            return new ApplyExpression(function, argument, higherOrder);
        }
    }
}
