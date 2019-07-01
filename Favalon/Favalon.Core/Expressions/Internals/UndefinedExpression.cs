using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions.Internals
{
    public sealed class UndefinedExpression : PseudoVariableExpression
    {
        private UndefinedExpression()
        { }

        public override string Name =>
            "?";

        protected override string FormatReadableString(FormatContext context) =>
            "?";

        public override string ToString() =>
            $"Undefined: ?";

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context) =>
            environment.CreatePlaceholder(Instance);

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context) =>
            (false, this);

        public static readonly UndefinedExpression Instance = new UndefinedExpression();
    }
}
