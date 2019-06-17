using System;

namespace Favalon.Expressions
{
    public sealed class IntegerExpression : Expression, IResolvedExpression
    {
        public readonly int Value;

        internal IntegerExpression(int value) =>
            this.Value = value;

        public override string ReadableString =>
            this.Value.ToString();

        public Expression HigherOrderExpression =>
            Type("System.Int32");

        public override Expression Infer(ExpressionEnvironment environment) =>
            this;
    }
}
