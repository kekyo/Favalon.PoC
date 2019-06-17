using System;

namespace Favalon.Expressions
{
    public sealed class ResolvedVariableExpression : VariableExpression, IResolvedExpression
    {
        internal ResolvedVariableExpression(string name, Expression higherOrderExpression) : base(name) =>
            this.HigherOrderExpression = higherOrderExpression;

        public Expression HigherOrderExpression { get; }

        public override Expression Infer(ExpressionEnvironment environment) =>
            this;
    }
}
