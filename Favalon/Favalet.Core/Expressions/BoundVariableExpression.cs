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

        public override bool IsAlwaysVisibleInAnnotation =>
           true;

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            this.VisitInferringImplicitVariable(
                environment,
                (name, higherOrder) => new BoundVariableExpression(name, higherOrder),
                higherOrderHint);

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var higherOrder = environment.Visit(this.HigherOrder);
            return new BoundVariableExpression(this.Name, higherOrder);
        }
    }
}
