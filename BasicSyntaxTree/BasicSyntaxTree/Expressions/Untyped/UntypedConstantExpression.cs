using BasicSyntaxTree.Expressions.Typed;
using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Untyped
{
    public sealed class UntypedConstantExpression : UntypedExpression
    {
        public readonly int Value;

        internal UntypedConstantExpression(int value) =>
            this.Value = value;

        internal override TypedExpression Visit(TypeEnvironment environment, VariableContext context) =>
            new ConstantExpression(this.Value, Type.Integer());

        public override string ToString() =>
            this.Value.ToString();
    }
}
