using Favalon.Expressions.Internals;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Favalon.Core.Tests")]

namespace Favalon.Expressions
{
    public abstract class Expression
    {
        private protected Expression(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public Expression HigherOrder { get; internal set; }

        protected internal virtual Expression Visit(Environment environment, InferContext context) =>
            this;

        protected internal virtual bool FixupChildren(InferContext context) =>
            false;

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

        public override string ToString() =>
            $"{this.GetType().Name.Replace("Expression", string.Empty)}: {this.GetInternalReadableString(true, GetInternalReadableString)}";

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator Expression(string name) =>
            new VariableExpression(name);
    }

    public static class ExpressionExtensions
    {
        public static TExpression Infer<TExpression>(this TExpression expression, Environment environment)
            where TExpression : Expression
        {
            var context = new InferContext();
            var visited = expression.Visit(environment, context);
            var fixup = context.Fixup(visited);
            return (TExpression)fixup;
        }
    }
}
