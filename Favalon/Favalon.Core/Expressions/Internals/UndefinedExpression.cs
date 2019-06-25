namespace Favalon.Expressions.Internals
{
    internal sealed class UndefinedExpression : Expression
    {
        // Dead end expression.

        internal UndefinedExpression() : base(null!, TextRange.Unknown)
        { }

        public override bool ShowInAnnotation =>
            false;

        protected internal override string FormatReadableString(FormatContext context) =>
            "(Undefined)";

        public static readonly UndefinedExpression Instance = new UndefinedExpression();

        static UndefinedExpression() =>
            Instance.HigherOrder = Instance;  // Infinite recursivity.
    }
}
