using BasicSyntaxTree.Typed.Expressions;
using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Untyped.Expressions
{
    public sealed class UntypedConstantExpression : UntypedExpression
    {
        public readonly object Value;

        internal UntypedConstantExpression(object value, TextRegion textRegion) : base(textRegion) =>
            this.Value = value;

        internal override TypedExpression Visit(TypeEnvironment environment, InferContext context) =>
            new ConstantExpression(this.Value, Type.ClsType(this.Value.GetType()), this.TextRegion);

        public override string ToString() =>
            this.Value.ToString();
    }
}
