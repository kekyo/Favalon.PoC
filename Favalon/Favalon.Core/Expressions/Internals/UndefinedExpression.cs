namespace Favalon.Expressions.Internals
{
    public sealed class UndefinedExpression :
        Expression<UndefinedExpression>
    {
        // Dead end expression.

        internal UndefinedExpression() : base(null!, TextRange.Unknown)
        { }

        private void Initialize() =>
            base.SetHigherOrder(Instance);  // Make infinite recursivity.

        public override bool ShowInAnnotation =>
            false;

        internal override void SetHigherOrder(Expression higherOrder) =>
            throw new System.InvalidOperationException($"Cannot annotate undefined: ?:{higherOrder}");

        protected internal override string FormatReadableString(FormatContext context) =>
            context.StrictNaming ? "(Undefined)" : "?";

        public static readonly UndefinedExpression Instance =
            new UndefinedExpression();

        static UndefinedExpression() =>
            Instance.Initialize();
    }
}
