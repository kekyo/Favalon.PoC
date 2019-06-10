using BasicSyntaxTree.Untyped;
using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree.Typed.Expressions
{
    public sealed class VariableExpression : TypedExpression
    {
        public readonly string Name;

        internal VariableExpression(string name, Type type, TextRegion textRegion) : base(type, textRegion) =>
            this.Name = name;

        internal override bool IsSafePrintable => false;

        internal override void Resolve(InferContext context) =>
            this.Type = context.ResolveType(this.Type);

        public override string ToString() =>
            $"{this.Name}:{this.Type}";
    }
}
