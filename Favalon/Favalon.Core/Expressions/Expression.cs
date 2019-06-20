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

        private string GetInternalReadableString(
            bool withAnnotation, System.Func<Expression, bool, string> getReadableString) =>
            (withAnnotation && !this.IsIgnoreAnnotationReadableString && !this.HigherOrder.IsIgnoreReadableString) ?
                $"{getReadableString(this, withAnnotation)}:{getReadableString(this.HigherOrder, false)}" :
                getReadableString(this, withAnnotation);

        private static string GetInternalReadableString(Expression expression, bool withAnnotation) =>
            expression.GetInternalReadableString(withAnnotation);

        private static string GetSafeInternalReadableString(Expression expression, bool withAnnotation) =>
            (expression.CanProduceSafeReadableString || !withAnnotation) ?
                expression.GetInternalReadableString(withAnnotation) :
                $"({expression.GetInternalReadableString(withAnnotation)})";

        public string GetReadableString(bool withAnnotation) =>
            GetInternalReadableString(withAnnotation, GetSafeInternalReadableString);

        public string ReadableString =>
            this.GetInternalReadableString(false, GetInternalReadableString);

        public Expression HigherOrder { get; internal set; }

        internal abstract Expression Visit(ExpressionEnvironment environment);
        internal abstract void Resolve(ExpressionEnvironment environment);

        public override string ToString() =>
            $"{this.GetType().Name.Replace("Expression", string.Empty)}: {this.GetInternalReadableString(true, GetInternalReadableString)}";

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator Expression(string name) =>
            new VariableExpression(name);
    }

    public static class ExpressionExtensions
    {
        public static TExpression Infer<TExpression>(this TExpression expression, ExpressionEnvironment environment)
            where TExpression : Expression
        {
            var visited = (TExpression)expression.Visit(environment);
            visited.Resolve(environment);
            return visited;
        }
    }
}
