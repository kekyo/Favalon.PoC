using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class FreeVariableExpression : SymbolicVariableExpression
    {
        internal FreeVariableExpression(string name, Expression higherOrder) :
            base(name, higherOrder)
        { }

        protected override Expression VisitInferring(Environment environment, Expression higherOrderHint)
        {
            if (Lookup(environment, this) is Expression bound)
            {
                var higherOrder = Unify(environment, higherOrderHint, this.HigherOrder, bound.HigherOrder);
                return new FreeVariableExpression(this.Name, higherOrder);
            }
            else
            {
                throw new InvalidOperationException($"Cannot find variable. Name={this.Name}");
            }
        }
    }
}
