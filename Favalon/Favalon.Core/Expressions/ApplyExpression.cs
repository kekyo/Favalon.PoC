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

        public override string ReadableString =>
            $"{this.Function} {this.Parameter}";

        protected override Expression VisitInferring(Environment environment, InferContext context)
        {
            var function = VisitInferring(environment, this.Function, context);
            var parameter = VisitInferring(environment, this.Parameter, context);
            var higherOrder = VisitInferringHigherOrder(environment, this.HigherOrder, context);
            return new ApplyExpression(function, parameter, higherOrder);
        }
    }
}
