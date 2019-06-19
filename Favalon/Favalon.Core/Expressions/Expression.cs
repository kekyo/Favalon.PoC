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

        internal abstract bool CanProduceSafeReadableString { get; }
        internal virtual bool IsIgnoreAnnotationReadableString =>
            false;
        internal virtual bool IsIgnoreReadableString =>
            false;

        internal abstract string GetInternalReadableString(bool withAnnotation);

        private static string GetSafeInternalReadableString(Expression expression, bool withAnnotation) =>
            expression.CanProduceSafeReadableString ?
                expression.GetInternalReadableString(withAnnotation) :
                $"({expression.GetInternalReadableString(withAnnotation)})";

        public string GetReadableString(bool withAnnotation) =>
            (withAnnotation && !this.IsIgnoreAnnotationReadableString && !this.HigherOrder.IsIgnoreReadableString) ?
                $"{GetSafeInternalReadableString(this, withAnnotation)}:{GetSafeInternalReadableString(this.HigherOrder, false)}" :
                GetSafeInternalReadableString(this, withAnnotation);

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
