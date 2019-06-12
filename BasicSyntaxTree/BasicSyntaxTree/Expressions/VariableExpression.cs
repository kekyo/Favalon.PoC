using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions
{
    public sealed class VariableExpression : ResolvedExpression
    {
        public readonly string Name;

        internal VariableExpression(string name, Type inferredType, TextRegion textRegion) :
            base(inferredType, textRegion) =>
            this.Name = name;

        internal override bool IsSafePrintable => false;

        internal override void Resolve(InferContext context) =>
            this.InferredType = context.ResolveType(this.InferredType);

        public override string ToString() =>
            (this.Name.Length >= 1) ? $"{this.Name}:{this.InferredType}" : this.InferredType.ToString();
    }
}
