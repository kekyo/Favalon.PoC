using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class KindExpression : Expression
    {
        private KindExpression() :
            base(UndefinedExpression.Instance)
        { }

        internal override bool CanProduceSafeReadableString =>
            true;
        internal override bool IsIgnoreAnnotationReadableString =>
            true;
        internal override bool IsIgnoreReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            "(Kind)";

        internal static readonly KindExpression Instance = new KindExpression();
    }
}
