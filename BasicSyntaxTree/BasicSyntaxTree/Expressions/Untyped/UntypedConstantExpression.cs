using BasicSyntaxTree.Expressions.Typed;
using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Untyped
{
    public sealed class UntypedConstantExpression : UntypedExpression
    {
        public readonly object Value;

        internal UntypedConstantExpression(object value) =>
            this.Value = value;

        internal override TypedExpression Visit(TypeEnvironment environment, InferContext context) =>
            new ConstantExpression(this.Value, Type.Dotnet(this.Value.GetType()));

        public override string ToString() =>
            this.Value.ToString();
    }
}
