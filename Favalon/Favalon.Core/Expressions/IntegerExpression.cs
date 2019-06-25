using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class IntegerExpression : TermExpression
    {
        public readonly int Value;

        internal IntegerExpression(int value, TextRange textRange) :
            base(Int32Type, textRange) =>
            this.Value = value;

        protected internal override string FormatReadableString(FormatContext context) =>
            this.Value.ToString();

        private static readonly TypeExpression Int32Type =
            new TypeExpression("System.Int32", TextRange.Unknown);
    }
}
