using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class ApplyExpression : TermExpression
    {
        public readonly TermExpression Function;
        public readonly TermExpression Parameter;

        internal ApplyExpression(TermExpression function, TermExpression parameter, TermExpression higherOrder) :
            base(higherOrder)
        {
            this.Function = function;
            this.Parameter = parameter;
        }

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context) =>
            ((this.Parameter is ApplyExpression) ?
                $"{FormatReadableString(context, this.Function, true)} ({FormatReadableString(context, this.Parameter, false)})" :
                $"{FormatReadableString(context, this.Function, true)} {FormatReadableString(context, this.Parameter, true)}",
            true);

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context)
        {
            var function = VisitInferring(environment, this.Function, context);
            var parameter = VisitInferring(environment, this.Parameter, context);
            var higherOrder = VisitInferringHigherOrder(environment, this.HigherOrder, context);

            var functionHigherOrder = new LambdaExpression(parameter.HigherOrder, higherOrder, UndefinedExpression.Instance);
            Unify(environment, function.HigherOrder, functionHigherOrder);

            return new ApplyExpression(function, parameter, higherOrder);
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context)
        {
            var (rf, function) = VisitResolving(environment, this.Function, context);
            var (rp, parameter) = VisitResolving(environment, this.Parameter, context);
            var (rho, higherOrder) = VisitResolvingHigherOrder(environment, this.HigherOrder, context);
            return (rf || rp || rho) ? (true, new ApplyExpression(function, parameter, higherOrder)) : (false, this);
        }
    }
}
