using BasicSyntaxTree.Untyped;
using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree.Typed.Expressions
{
    public sealed class VariableExpression : TypedExpression
    {
        public readonly string Name;

        internal VariableExpression(string name, UntypedType type, TextRegion textRegion) : base(type, textRegion) =>
            this.Name = name;

        internal override void Resolve(InferContext context) =>
            this.Type = context.ResolveType(this.Type);

        public override string ToString() =>
            $"{this.Name}:{this.Type}";
    }
}
