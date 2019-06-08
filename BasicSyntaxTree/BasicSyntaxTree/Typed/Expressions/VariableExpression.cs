using BasicSyntaxTree.Types;
using BasicSyntaxTree.Untyped;

namespace BasicSyntaxTree.Typed.Expressions
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
