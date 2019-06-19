using System.Diagnostics;
using System.Runtime.CompilerServices;

using Favalon.Expressions.Internals;

[assembly:InternalsVisibleTo("Favalon.Core.Tests")]

namespace Favalon.Expressions
{
    public abstract class Expression
    {
        private protected Expression(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        internal abstract string GetInternalReadableString(bool withAnnotation);

        public string GetReadableString(bool withAnnotation) =>
            (withAnnotation && !(this.HigherOrder is UndefinedExpression)) ?
                $"{this.GetInternalReadableString(withAnnotation)}:{this.HigherOrder.GetInternalReadableString(false)}" :
                this.GetInternalReadableString(withAnnotation);

        public string ReadableString =>
            this.GetReadableString(true);

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
