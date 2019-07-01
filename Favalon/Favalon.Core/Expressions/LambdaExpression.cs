using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class LambdaExpression : ValueExpression
    {
        public readonly TermExpression Parameter;
        public readonly TermExpression Expression;

        internal LambdaExpression(TermExpression parameter, TermExpression expression, TermExpression higherOrder) :
            base(higherOrder)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        protected override string FormatReadableString(FormatContext context) =>
            context.FancySymbols ?
                $"{FormatReadableString(context, this.Parameter)} → {FormatReadableString(context, this.Expression)}" :
                $"{FormatReadableString(context, this.Parameter)} -> {FormatReadableString(context, this.Expression)}";

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context)
        {
            var newScope = environment.NewScope();

            VariableExpression parameter;
            if (this.Parameter is PseudoVariableExpression)
            {
                parameter = environment.CreatePlaceholder(UndefinedExpression.Instance);
                newScope.Register(parameter);
            }
            else if (this.Parameter is VariableExpression variable)
            {
                newScope.Register(variable);
                parameter = VisitInferring(newScope, variable, context);
            }
            else
            {
                throw new ArgumentException("Invalid lambda parameter expression.", this.Parameter.ReadableString);
            }

            var expression = VisitInferring(newScope, this.Expression, context);
            var higherOrder = VisitInferringHigherOrder(newScope, this.HigherOrder, context);

            return new LambdaExpression(parameter, expression, higherOrder);
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context)
        {
            var (rp, parameter) = VisitResolving(environment, this.Parameter, context);
            var (re, expression) = VisitResolving(environment, this.Expression, context);
            var (rho, higherOrder) = VisitResolvingHigherOrder(environment, this.HigherOrder, context);

            return (rp || re || rho) ? (true, new LambdaExpression(parameter, expression, higherOrder)) : (false, this);
        }
    }
}
