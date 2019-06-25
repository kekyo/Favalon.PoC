using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class ConstantExpression : TermExpression
    {
        public readonly object Value;

        internal ConstantExpression(object value, TextRange textRange) :
            base(Type(value.GetType(), textRange), textRange) =>
            this.Value = value;

        protected internal override string FormatReadableString(FormatContext context) =>
            this.Value is string ? $"\"{this.Value}\"" : this.Value.ToString();
    }
}
