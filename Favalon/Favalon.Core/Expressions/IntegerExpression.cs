using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class IntegerExpression : TermExpression
    {
        public readonly int Value;

        internal IntegerExpression(int value) :
            base(Int32Type) =>
            this.Value = value;

        protected internal override string FormatReadableString(FormatStringContext context) =>
            this.Value.ToString();

        private static readonly TypeExpression Int32Type = new TypeExpression("System.Int32");
    }
}
