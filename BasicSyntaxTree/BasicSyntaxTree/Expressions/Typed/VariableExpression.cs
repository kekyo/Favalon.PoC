using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Typed
{
    public sealed class VariableExpression : TypedExpression
    {
        public readonly string Name;

        internal VariableExpression(string name, Type type, TextRegion textRegion) : base(type, textRegion) =>
            this.Name = name;

        public override string ToString() =>
            $"{this.Name}:{this.Type}";
    }
}
