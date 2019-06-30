using Favalon.Expressions.Internals;
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

        protected override string FormatReadableString(FormatContext context) =>
            this.TypeName;

        protected override Expression VisitInferring(Environment environment, InferContext context) =>
            this;

        protected override (bool isResolved, Expression resolved) VisitResolving(Environment environment, InferContext context) =>
            (false, this);
    }
}
