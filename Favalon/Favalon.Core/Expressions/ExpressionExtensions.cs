using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public static class ExpressionExtensions
    {
        private static string InternalFormatReadableString(this Expression expression, bool withAnnotation) =>
            (expression is TermExpression) ?
                expression.FormatReadableString(withAnnotation) :
                $"({expression.FormatReadableString(withAnnotation)})";

        public static string GetReadableString(this Expression expression, bool withAnnotation) =>
            (withAnnotation && expression.HigherOrder.ShowInAnnotation) ?
                $"{expression.InternalFormatReadableString(true)}:{expression.HigherOrder.InternalFormatReadableString(false)}" :
                expression.FormatReadableString(false);

        public static Expression Infer(this Expression expression, Environment environment, int rank = 0)
        {
            var context = new InferContext();
            var visited = expression.Visit(environment, context);
            var fixup = context.FixupHigherOrders(visited, rank);
            var aggregated = context.AggregatePlaceholders(fixup, rank);
            context.RearrangePlaceholderIndex();
            return aggregated;
        }
    }
}
