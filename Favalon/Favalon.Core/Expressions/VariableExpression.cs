using Favalon.Expressions.Internals;

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

        internal override string GetInternalReadableString(bool withAnnotation) =>
            this.Name.ToString();

        internal override Expression Visit(ExpressionEnvironment environment)
        {
            if (environment.TryGetHigherOrder(this.Name, out var higherOrder))
            {
                return new VariableExpression(this.Name, higherOrder);
            }

            var placeholder = environment.CreatePlaceholder();
            environment.SetHigherOrder(this.Name, placeholder);
            return new VariableExpression(this.Name, placeholder);
        }

        internal override void Resolve(ExpressionEnvironment environment) =>
            this.HigherOrder = environment.Resolve(this.HigherOrder);

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator VariableExpression(string name) =>
            new VariableExpression(name);
    }
}
