using Favalon.Expressions.Internals;
using System.Linq;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public sealed class ReadableStringContext
    {
        public readonly bool WithAnnotation;
        public readonly bool Fancy;

        internal ReadableStringContext(bool withAnnotation, bool fancy)
        {
            this.WithAnnotation = withAnnotation;
            this.Fancy = fancy;
        }

        public ReadableStringContext DisableAnnotation() =>
            new ReadableStringContext(false, this.Fancy);
    }

    public static class Extensions
    {
        public static string GetExpressionShortName(this Expression expression) =>
            expression.GetType().Name.Replace("Expression", string.Empty);

        private static string InternalFormatReadableString(this Expression expression, ReadableStringContext context) =>
            (expression is TermExpression) ?
                expression.FormatReadableString(context) :
                $"({expression.FormatReadableString(context)})";

        public static string GetReadableString(this Expression expression, ReadableStringContext context) =>
            (context.WithAnnotation && expression.HigherOrder.ShowInAnnotation) ?
                $"{expression.InternalFormatReadableString(context)}:{expression.HigherOrder.InternalFormatReadableString(context.DisableAnnotation())}" :
                expression.FormatReadableString(context.DisableAnnotation());

        public static string GetReadableString(this Expression expression, bool withAnnotation, bool fancy = false) =>
            expression.GetReadableString(new ReadableStringContext(withAnnotation, fancy));

        public static XElement CreateXml(this Expression expression, bool strictAnnotation) =>
            new XElement(expression.GetExpressionShortName(),
                new[] { (XObject)new XAttribute("expression", expression.GetReadableString(false, true)) }.
                Concat(strictAnnotation ?
                    new[] { (XObject)new XElement("HigherOrder", expression.HigherOrder.CreateXml(strictAnnotation)) } :
                    expression.HigherOrder.ShowInAnnotation ?
                        new[] { (XObject)new XAttribute("higherOrder", expression.HigherOrder.GetReadableString(false, true)) } :
                        Enumerable.Empty<XObject>()).
                Concat(expression.CreateXmlChildren(strictAnnotation)).
                ToArray<object>());

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
