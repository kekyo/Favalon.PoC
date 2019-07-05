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
            this.VisitInferringImplicitly(environment, higherOrderHint);
    }
}
