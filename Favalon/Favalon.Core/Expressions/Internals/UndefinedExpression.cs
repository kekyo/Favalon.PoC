namespace Favalon.Expressions.Internals
{
    internal sealed class UndefinedExpression : Expression
    {
        // Dead end expression.

        internal UndefinedExpression() :
            base(null!)
        { }

        internal override bool CanProduceSafeReadableString =>
            true;
        internal override bool IsIgnoreAnnotationReadableString =>
            true;
        internal override bool IsIgnoreReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            "(Undefined)";

        internal override Expression Visit(ExpressionEnvironment environment, InferContext context) =>
            this;
        internal override Expression FixupChildren(InferContext context) =>
            this;

        internal static readonly UndefinedExpression Instance = new UndefinedExpression();

        static UndefinedExpression() =>
            Instance.HigherOrder = Instance;  // Infinite recursivity.
    }
}
