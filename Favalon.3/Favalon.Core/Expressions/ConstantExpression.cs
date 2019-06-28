using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class ConstantExpression : TermExpression
    {
        public readonly object Value;

        internal ConstantExpression(object value, TextRange textRange) :
            base(Type(value.GetType(), textRange), textRange) =>
            this.Value = value;

        internal override void SetHigherOrder(Expression higherOrder) =>
            throw new System.InvalidOperationException($"Cannot annotate constant: {this}:{higherOrder}");

        protected internal override string FormatReadableString(FormatContext context) =>
            this.Value is string ? $"\"{this.Value}\"" : this.Value.ToString();
    }
}
