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

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context, TermExpression higherOrderHint)
        {
            var newScope = environment.NewScope();

            var expression = VisitInferring(newScope, context, this.Expression, this.Bound.HigherOrder);
            var bound = VisitInferring(newScope, context, this.Bound, UndefinedExpression.Instance);
            Unify___(newScope, bound.HigherOrder, expression.HigherOrder);

            newScope.SetBoundExpression(bound, expression);

            var body = VisitInferring(newScope, context, this.Body, higherOrderHint);
            var higherOrder = VisitInferring(newScope, context, this.HigherOrder, higherOrderHint);
            Unify___(newScope, higherOrder, body.HigherOrder);

            return new BindExpression(bound, expression, body, higherOrder);
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context)
        {
            var (re, expression) = VisitResolving(environment, this.Expression, context);
            var (rb, bound) = VisitResolving(environment, this.Bound, context);
            var (r, body) = VisitResolving(environment, this.Body, context);
            var (rho, higherOrder) = VisitResolving(environment, this.HigherOrder, context);

            return (rb || re || r || rho) ? (true, new BindExpression(bound, expression, body, higherOrder)) : (false, this);
        }
    }
}
