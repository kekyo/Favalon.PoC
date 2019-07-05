using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public abstract class PseudoExpression : Expression
    {
        private protected PseudoExpression() :
            base(null!)
        { }

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            throw new NotImplementedException();

        protected override Expression VisitResolving(IResolvingEnvironment environment) =>
            this;
    }
}
