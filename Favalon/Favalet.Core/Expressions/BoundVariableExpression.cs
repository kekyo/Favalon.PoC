using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class BoundVariableExpression : SymbolicVariableExpression
    {
        internal BoundVariableExpression(string name, Expression higherOrder) :
            base(name, higherOrder)
        { }

        protected override Expression VisitInferring(Environment environment, Expression higherOrderHint) =>
            this.VisitInferringImplicitly(
                environment,
                (name, higherOrder) => new BoundVariableExpression(name, higherOrder),
                higherOrderHint);

        protected override Expression VisitResolving(Environment environment)
        {
            var higherOrder = VisitResolving(environment, this.HigherOrder);
            return new BoundVariableExpression(this.Name, higherOrder);
        }
    }
}
