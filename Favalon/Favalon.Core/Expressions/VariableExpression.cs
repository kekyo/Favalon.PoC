using System;

namespace Favalon.Expressions
{
    public sealed class VariableExpression : Expression
    {
        public readonly string Name;

        internal VariableExpression(string name) :
            base(UndefinedExpression.Instance) =>
            this.Name = name;

        private VariableExpression(string name, Expression higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        public override string ReadableString =>
            this.Name.ToString();

        internal override Expression Visit(ExpressionEnvironment environment)
        {
            if (environment.TryGetVariable(this.Name, out var expression))
            {
                return new VariableExpression(this.Name, expression.HigherOrder);
            }

            var placeholder = environment.CreatePlaceholder();
            environment.AddVariable(this.Name, placeholder);
            return new VariableExpression(this.Name, placeholder);
        }

        internal override void Resolve(ExpressionEnvironment environment) =>
            this.HigherOrder = environment.Resolve(this.HigherOrder);

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator VariableExpression(string name) =>
            new VariableExpression(name);
    }
}
