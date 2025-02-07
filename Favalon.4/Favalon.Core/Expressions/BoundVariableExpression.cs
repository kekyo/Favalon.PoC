﻿using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class BoundVariableExpression : VariableExpression
    {
        internal BoundVariableExpression(string name, TermExpression higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        public override string Name { get; }

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context) =>
            (this.Name, false);

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context)
        {
            var higherOrder = VisitInferring(environment, this.HigherOrder, context);
            return new BoundVariableExpression(this.Name, higherOrder);
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context)
        {
            var (rho, higherOrder) = VisitResolving(environment, this.HigherOrder, context);
            return rho ? (true, new BoundVariableExpression(this.Name, higherOrder)) : (false, this);
        }
    }
}
