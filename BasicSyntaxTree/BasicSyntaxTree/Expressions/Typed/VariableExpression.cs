using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Typed
{
    public sealed class VariableExpression : TypedExpression
    {
        public readonly string Name;

        internal VariableExpression(string name, Type type, TextRegion textRegion) : base(type, textRegion) =>
            this.Name = name;

        internal override void Resolve(InferContext context) =>
            this.Type = context.ResolveType(this.Type);

        public override string ToString() =>
            $"{this.Name}:{this.Type}";
    }
}
