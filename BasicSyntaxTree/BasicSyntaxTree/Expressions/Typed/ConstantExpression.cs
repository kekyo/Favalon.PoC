using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Typed
{
    public sealed class ConstantExpression : TypedExpression
    {
        public readonly object Value;

        internal ConstantExpression(object value, Type type, TextRegion textRegion) : base(type, textRegion) =>
            this.Value = value;

        public override string ToString() =>
            this.Value is string str ? $"\"{str}\":{this.Type}" : $"{this.Value}:{this.Type}";
    }
}
