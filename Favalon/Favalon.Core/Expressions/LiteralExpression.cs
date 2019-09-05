namespace Favalon.Expressions
{
    public abstract class LiteralExpression : Expression
    {
        private protected LiteralExpression()
        { }

        public abstract object RawValue { get; }

        public override string ToString() =>
            this.RawValue?.ToString() ?? "(null)";
    }

    public class LiteralExpression<TValue> : LiteralExpression
    {
        private static readonly Expression higherOrder =
            new VariableExpression(typeof(TValue).FullName, UnspecifiedExpression.Instance);

        public readonly TValue Value;

        public LiteralExpression(TValue value)
            : base() =>
            this.Value = value;

        public override Expression HigherOrder =>
            higherOrder;

        public override object RawValue =>
            this.Value!;

        protected override Expression VisitResolve(IInferContext context) =>
            this;

        public override bool Equals(Expression? rhs) =>
            rhs is LiteralExpression<TValue> literal &&
            (this.Value?.Equals(literal.Value) ?? false);
    }
}
