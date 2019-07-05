﻿using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Internals
{
    public sealed class ImplicitVariableExpression : SymbolicVariableExpression
    {
        internal ImplicitVariableExpression(string name, Expression higherOrder) :
            base(name, higherOrder)
        { }

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            this.VisitInferringImplicitly(
                environment,
                (name, higherOrder) => new ImplicitVariableExpression(name, higherOrder),
                higherOrderHint);

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var higherOrder = environment.Visit(this.HigherOrder);
            return new ImplicitVariableExpression(this.Name, higherOrder);
        }
    }
}
