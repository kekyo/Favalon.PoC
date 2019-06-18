using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Favalon.Core.Tests")]

namespace Favalon.Expressions
{
    public abstract class Expression
    {
        private protected Expression(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public abstract string ReadableString { get; }

        public Expression HigherOrder { get; internal set; }

        internal abstract Expression Visit(ExpressionEnvironment environment);
        internal abstract void Resolve(ExpressionEnvironment environment);

        public Expression Infer(ExpressionEnvironment environment)
        {
            var expression = this.Visit(environment);
            expression.Resolve(environment);
            return expression;
        }

        public override string ToString() =>
            $"{this.GetType().Name.Replace("Expression", string.Empty)}: {this.ReadableString}";

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator Expression(string name) =>
            new VariableExpression(name);
    }
}
