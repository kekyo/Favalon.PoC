﻿using Favalon.Expressions.Internals;
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

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context)
        {
            var arrow = context.FancySymbols ? "→" : "->";
            return ($"{FormatReadableString(context, this.Parameter, true)} {arrow} {FormatReadableString(context, this.Expression, false)}", true);
        }

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context)
        {
            var newScope = environment.NewScope();

            // Undefined or Kind
            VariableExpression parameter;
            switch (this.Parameter)
            {
                case UndefinedExpression _:
                    parameter = environment.CreatePlaceholder(UndefinedExpression.Instance);
                    newScope.SetBoundExpression(parameter, parameter);
                    break;
                case KindExpression _:
                    parameter = KindExpression.Instance;
                    break;
                case VariableExpression variable:
                    newScope.SetBoundExpression(variable, variable);
                    parameter = VisitInferring(newScope, variable, context);
                    break;
                default:
                    throw new ArgumentException("Invalid lambda parameter expression.", this.Parameter.ReadableString);
            }

            var expression = VisitInferring(newScope, this.Expression, context);

            var higherOrder = VisitInferring(newScope, this.HigherOrder, context);
            var newHigherOrder = new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, higherOrder.HigherOrder);
            Unify___(newScope, higherOrder, newHigherOrder);

            return new LambdaExpression(parameter, expression, higherOrder);
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context)
        {
            var (rp, parameter) = VisitResolving(environment, this.Parameter, context);
            var (re, expression) = VisitResolving(environment, this.Expression, context);
            var (rho, higherOrder) = VisitResolving(environment, this.HigherOrder, context);

            return (rp || re || rho) ? (true, new LambdaExpression(parameter, expression, higherOrder)) : (false, this);
        }
    }
}
