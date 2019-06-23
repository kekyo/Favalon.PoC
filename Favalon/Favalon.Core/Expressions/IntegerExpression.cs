namespace Favalon.Expressions
{
    public sealed class IntegerExpression : TermExpression
    {
        public readonly int Value;

        internal IntegerExpression(int value) :
            base(Int32Type) =>
            this.Value = value;

        protected override string FormatReadableString(bool withAnnotation) =>
            this.Value.ToString();

        private static readonly TypeExpression Int32Type = new TypeExpression("System.Int32");
    }
}
