using System;

namespace Favalon.Expressions
{
    public sealed class TypeExpression : Expression, IResolvedExpression
    {
        public readonly string Name;

        internal TypeExpression(string name) =>
            this.Name = name;

        public override string ReadableString =>
            this.Name.ToString();

        public Expression HigherOrderExpression =>
            Kind();

        public override Expression Infer(ExpressionEnvironment environment) =>
            this;
    }
}
