using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Typed
{
    public sealed class ConstantExpression : TypedExpression
    {
        public readonly int Value;

        internal ConstantExpression(int value, Type type) : base(type) =>
            this.Value = value;

        public override string ToString() =>
            $"{this.Value}:{this.Type}";
    }
}
