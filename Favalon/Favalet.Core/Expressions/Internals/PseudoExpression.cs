using System;

namespace Favalet.Expressions.Internals
{
    public abstract class PseudoExpression : Expression
    {
        private protected PseudoExpression(Expression higherOrder) :
            base(higherOrder)
        { }

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            throw new InvalidOperationException();

        protected override Expression VisitResolving(IResolvingEnvironment environment) =>
            this;
    }
}
