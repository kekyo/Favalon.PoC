using System;

namespace Favalon.Expressions
{
    public sealed class UnresolvedVariableExpression : VariableExpression
    {
        internal UnresolvedVariableExpression(string name) : base(name) { }

        public override Expression Infer(ExpressionEnvironment environment) =>
            this;
    }
}
