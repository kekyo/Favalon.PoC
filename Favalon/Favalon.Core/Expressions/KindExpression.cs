using System;

namespace Favalon.Expressions
{
    public sealed class KindExpression : Expression
    {
        private KindExpression() :
            base(UndefinedExpression.Instance)
        { }

        public override string ReadableString =>
            "(Kind)";

        internal override Expression Visit(ExpressionEnvironment environment) =>
            this;

        internal override void Resolve(ExpressionEnvironment environment)
        {
        }

        internal static readonly KindExpression Instance = new KindExpression();
    }
}
