﻿using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class TypeExpression : TermExpression
    {
        public readonly string TypeName;

        internal TypeExpression(string typeName) :
            base(KindExpression.Instance) =>
            this.TypeName = typeName;

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context) =>
            (this.TypeName, false);

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context) =>
            this;

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context) =>
            (false, this);
    }
}
