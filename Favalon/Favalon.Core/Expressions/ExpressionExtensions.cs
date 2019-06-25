using Favalon.Expressions.Internals;
using System.Linq;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public static class ExpressionExtensions
    {
        public static string GetExpressionShortName(this Expression expression) =>
            expression.GetType().Name.Replace("Expression", string.Empty);

        private static string InternalFormatReadableString(this Expression expression, FormatStringContext context) =>
            (expression is TermExpression) ?
                expression.FormatReadableString(context) :
                $"({expression.FormatReadableString(context)})";

        public static string GetReadableString(this Expression expression, FormatStringContext context) =>
            (context.WithAnnotation && expression.HigherOrder.ShowInAnnotation) ?
                $"{expression.InternalFormatReadableString(context)}:{expression.HigherOrder.InternalFormatReadableString(context.NewDerived(false, null))}" :
                expression.FormatReadableString(context.NewDerived(false, null));

        public static string GetReadableString(this Expression expression, bool withAnnotation, bool strictNaming, bool fancySymbols) =>
            expression.GetReadableString(new FormatStringContext(withAnnotation, strictNaming, fancySymbols));

        public static XElement CreateXml(this Expression expression, FormatStringContext context) =>
            new XElement(expression.GetExpressionShortName(),
                new[] { (XObject)new XAttribute("expression", expression.GetReadableString(context.NewDerived(false, true))) }.
                Concat(expression.HigherOrder.ShowInAnnotation ?
                    (context.WithAnnotation ?
                        new[] { (XObject)new XElement("HigherOrder", expression.HigherOrder.CreateXml(context)) } :
                        new[] { (XObject)new XAttribute("higherOrder", expression.HigherOrder.GetReadableString(context.NewDerived(false, true))) }) :
                    Enumerable.Empty<XObject>()).
                Concat(expression.CreateXmlChildren(context)).
                ToArray<object>());

        public static TExpression Infer<TExpression>(this TExpression expression, ExpressionEnvironment environment, int rank = 0)
            where TExpression : Expression
        {
            var context = new InferContext();

            // (visited is partially mutable in this sequence.)
            var visited = expression.VisitInferring(environment, context);
            var fixup = context.FixupHigherOrders(visited, rank);
            // (fixup is immutable)

            return (TExpression)fixup;
        }
    }
}
