using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class BindExpression : TermExpression
    {
        public readonly VariableExpression Bound;
        public readonly TermExpression Expression;
        public readonly TermExpression Body;

        internal BindExpression(VariableExpression bound, TermExpression expression, TermExpression body, TermExpression higherOrder) :
            base(higherOrder)
        {
            this.Bound = bound;
            this.Expression = expression;
            this.Body = body;
        }

        public override string ReadableString =>
            $"{this.Bound} = {this.Expression} in {this.Body}";

        protected override Expression VisitInferring(Environment environment)
        {
            var newScope = environment.NewScope();

            var (bound, expression) = newScope.InternalBind(this.Bound, this.Expression);
            var body = VisitInferring(newScope, this.Body);
            var higherOrder = VisitInferring(newScope, this.HigherOrder);

            return new BindExpression(bound, expression, body, higherOrder);
        }
    }
}
