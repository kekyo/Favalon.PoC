using System;

namespace Favalon.Expressions
{
    public sealed class KindExpression : Expression, IResolvedExpression
    {
        internal KindExpression() { }

        public override string ReadableString =>
            "Kind";

        public Expression HigherOrderExpression =>
            throw new NotImplementedException();

        public override Expression Infer(ExpressionEnvironment environment) =>
            this;
    }
}
