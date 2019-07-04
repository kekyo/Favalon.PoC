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
            var higherOrder = Unify(environment, higherOrderHint, this.HigherOrder);

            var argument = VisitInferring(environment, this.Argument, UndefinedExpression.Instance);
            var function = VisitInferring(environment, this.Function,
                new LambdaExpression(argument.HigherOrder, higherOrder, UndefinedExpression.Instance));

            return new ApplyExpression(function, argument, higherOrder);
        }

        protected override Expression VisitResolving(Environment environment)
        {
            var argument = VisitResolving(environment, this.Argument);
            var function = VisitResolving(environment, this.Function);
            var higherOrder = VisitResolving(environment, this.HigherOrder);

            return new ApplyExpression(function, argument, higherOrder);
        }
    }
}
