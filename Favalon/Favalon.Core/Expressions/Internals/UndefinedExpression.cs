namespace Favalon.Expressions.Internals
{
    internal sealed class UndefinedExpression : Expression
    {
        // Dead end expression.

        internal UndefinedExpression() : base(null!)
        { }

        internal override bool CanProduceSafeReadableString =>
            true;
        internal override bool IsIgnoreAnnotationReadableString =>
            true;
        internal override bool IsIgnoreReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            "(Undefined)";

        public static readonly UndefinedExpression Instance = new UndefinedExpression();

        static UndefinedExpression() =>
            Instance.HigherOrder = Instance;  // Infinite recursivity.
    }
}
