using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree.Expressions.Unresolved
{
    public sealed class UntypedConstantExpression : UntypedExpression
    {
        public readonly object Value;

        internal UntypedConstantExpression(object value, TextRegion textRegion) : base(new UntypedClrType(value.GetType()), textRegion) =>
            this.Value = value;

        internal override bool IsSafePrintable => true;

        internal override TypedExpression Visit(Environment environment, InferContext context) =>
            new ConstantExpression(this.Value, this.AnnotetedType!, this.TextRegion);

        public override string ToString() =>
            this.Value is string str ? $"\"{str}\"" : this.Value.ToString();
    }
}
