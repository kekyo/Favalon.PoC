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

        public override string ReadableString =>
            $"{this.Parameter} -> {this.Expression}";

        protected override Expression VisitInferring(Environment environment, InferContext context)
        {
            var newScope = environment.NewScope();

            var parameter = VisitInferring(newScope, this.Parameter, context);
            var expression = VisitInferring(newScope, this.Expression, context);
            var higherOrder = VisitInferringHigherOrder(newScope, this.HigherOrder, context);

            return new LambdaExpression(parameter, expression, higherOrder);
        }
    }
}
