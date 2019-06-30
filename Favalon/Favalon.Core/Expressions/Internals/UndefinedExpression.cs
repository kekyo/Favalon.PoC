using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions.Internals
{
    public sealed class UndefinedExpression : TermExpression
    {
        private UndefinedExpression() :
            base(null!)
        { }

        protected override string FormatReadableString(FormatContext context) =>
            "?";

        public override string ToString() =>
            $"Undefined: ?";

        protected override Expression VisitInferring(Environment environment, InferContext context) =>
            environment.CreatePlaceholder(Instance);

        protected override (bool isResolved, Expression resolved) VisitResolving(Environment environment, InferContext context) =>
            (false, this);

        public static readonly UndefinedExpression Instance = new UndefinedExpression();
    }
}
