namespace Favalon.Expressions.Internals
{
    internal sealed class UndefinedExpression : Expression
    {
        // Dead end expression.

        internal UndefinedExpression() : base(null!)
        { }

        public override bool ShowInAnnotation =>
            false;

        protected internal override string FormatReadableString(ReadableStringContext context) =>
            "(Undefined)";

        public static readonly UndefinedExpression Instance = new UndefinedExpression();

        static UndefinedExpression() =>
            Instance.HigherOrder = Instance;  // Infinite recursivity.
    }
}
