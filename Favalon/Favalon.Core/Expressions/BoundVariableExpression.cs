using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class BoundVariableExpression : VariableExpression
    {
        internal BoundVariableExpression(string name, TermExpression higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        public override string Name { get; }

        protected override string FormatReadableString(FormatContext context) =>
            this.Name;

        protected override Expression VisitInferring(Environment environment, InferContext context)
        {
            var higherOrder = VisitInferringHigherOrder(environment, this.HigherOrder, context);
            return new BoundVariableExpression(this.Name, higherOrder);
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(Environment environment, InferContext context)
        {
            var (rho, higherOrder) = VisitResolvingHigherOrder(environment, this.HigherOrder, context);
            return rho ? (true, new BoundVariableExpression(this.Name, higherOrder)) : (false, this);
        }
    }
}
