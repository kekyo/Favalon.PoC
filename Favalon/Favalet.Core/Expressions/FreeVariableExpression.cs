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

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint)
        {
            if (environment.Lookup(this) is Expression bound)
            {
                var higherOrder = environment.Unify(higherOrderHint, this.HigherOrder, bound.HigherOrder);
                return new FreeVariableExpression(this.Name, higherOrder);
            }
            else
            {
                throw new ArgumentException($"Cannot find variable: Name={this.Name}");
            }
        }

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var higherOrder = environment.Visit(this.HigherOrder);
            return new FreeVariableExpression(this.Name, higherOrder);
        }
    }
}
