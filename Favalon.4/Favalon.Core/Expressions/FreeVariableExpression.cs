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

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context) =>
            (this.Name, false);

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context, TermExpression higherOrderHint)
        {
            // NOTE: Inferring process will resolve only higher order information.
            //   It'll not process for reducing.
            if (environment.GetBoundHigherOrder(this) is TermExpression termHigherOrder)
            {
                termHigherOrder = VisitInferring(environment, termHigherOrder, context);
                var higherOrder = VisitInferring(environment, this.HigherOrder, context);
                Unify___(environment, termHigherOrder, higherOrder);

                return new FreeVariableExpression(this.Name, termHigherOrder);
            }
            else
            {
                throw new ArgumentException($"Couldn't resolve variable.", this.Name);
            }
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context)
        {
            var (rho, higherOrder) = VisitResolving(environment, this.HigherOrder, context);
            return rho ? (true, new FreeVariableExpression(this.Name, higherOrder)) : (false, this);
        }
    }
}
