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

        public override string ReadableString =>
            $"{this.Bound} = {this.Expression} in {this.Body}";

        protected override Expression VisitInferring(Environment environment, InferContext context)
        {
            var newScope = environment.NewScope();

            var (bound, expression) = newScope.InternalBind(this.Bound, this.Expression, context);
            var body = VisitInferring(newScope, this.Body, context);
            var higherOrder = VisitInferringHigherOrder(newScope, this.HigherOrder, context);

            return new BindExpression(bound, expression, body, higherOrder);
        }
    }
}
