using System;

namespace Favalon.Expressions
{
    public abstract class VariableExpression : Expression
    {
        public readonly string Name;

        private protected VariableExpression(string name) =>
            this.Name = name;

        public override string ReadableString =>
            this.Name.ToString();

        public override Expression Infer(ExpressionEnvironment environment) =>
            this;
    }
}
