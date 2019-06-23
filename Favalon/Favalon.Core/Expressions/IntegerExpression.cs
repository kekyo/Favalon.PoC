namespace Favalon.Expressions
{
    public sealed class IntegerExpression : TermExpression
    {
        public readonly int Value;

        internal IntegerExpression(int value) :
            base(Int32Type) =>
            this.Value = value;

        internal override bool CanProduceSafeReadableString =>
            true;
        internal override bool IsIgnoreAnnotationReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            this.Value.ToString();

        private static readonly TypeExpression Int32Type = new TypeExpression("System.Int32");
    }
}
