using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class BindExpression : TermExpression
    {
        public readonly BoundVariableExpression Bound;
        public readonly TermExpression Expression;
        public readonly TermExpression Body;

        internal BindExpression(BoundVariableExpression bound, TermExpression expression, TermExpression body, TermExpression higherOrder) :
            base(higherOrder)
        {
            this.Bound = bound;
            this.Expression = expression;
            this.Body = body;
        }

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context) =>
            ($"{FormatReadableString(context, this.Bound, false)} = {FormatReadableString(context, this.Expression, false)} in {FormatReadableString(context, this.Body, false)}", true);

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context)
        {
            var newScope = environment.NewScope();

            var expression = VisitInferring(newScope, this.Expression, context);
            var bound = VisitInferring(newScope, this.Bound, context);
            Unify(newScope, bound.HigherOrder, expression.HigherOrder);

            newScope.SetBoundExpression(bound, expression);

            var body = VisitInferring(newScope, this.Body, context);
            var higherOrder = VisitInferringHigherOrder(newScope, this.HigherOrder, context);
            Unify(newScope, body.HigherOrder, higherOrder);

            return new BindExpression(bound, expression, body, body.HigherOrder);
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context)
        {
            var (re, expression) = VisitResolving(environment, this.Expression, context);
            var (rb, bound) = VisitResolving(environment, this.Bound, context);
            var (r, body) = VisitResolving(environment, this.Body, context);
            var (rho, higherOrder) = VisitResolvingHigherOrder(environment, this.HigherOrder, context);

            return (rb || re || r || rho) ? (true, new BindExpression(bound, expression, body, higherOrder)) : (false, this);
        }
    }
}
