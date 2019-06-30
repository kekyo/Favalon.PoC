using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public class FreeVariableExpression : VariableExpression
    {
        internal FreeVariableExpression(string name, TermExpression higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        public override string Name { get; }

        protected override string FormatReadableString(FormatContext context) =>
            this.Name;

        protected override Expression VisitInferring(Environment environment, InferContext context)
        {
            // NOTE: Inferring process will resolve only higher order information.
            //   It'll not process for reducing.
            if (environment.GetBoundExpression(this) is TermExpression term)
            {
                var termHigherOrder = VisitInferringHigherOrder(environment, term.HigherOrder, context);
                var higherOrder = VisitInferringHigherOrder(environment, this.HigherOrder, context);
                Unify(environment, termHigherOrder, higherOrder);
                return new FreeVariableExpression(this.Name, termHigherOrder);
            }
            else
            {
                throw new ArgumentException($"Couldn't resolve variable.", this.Name);
            }
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(Environment environment, InferContext context)
        {
            var (rho, higherOrder) = VisitResolvingHigherOrder(environment, this.HigherOrder, context);
            return rho ? (true, new FreeVariableExpression(this.Name, higherOrder)) : (false, this);
        }
    }
}
