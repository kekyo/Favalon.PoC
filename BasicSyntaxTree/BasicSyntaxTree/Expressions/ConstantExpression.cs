using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions
{
    public sealed class ConstantExpression : ResolvedExpression
    {
        public readonly object Value;

        internal ConstantExpression(object value, Type inferredType, TextRegion textRegion) :
            base(inferredType, textRegion) =>
            this.Value = value;

        internal override bool IsSafePrintable => this.Value is string;

        internal override void Resolve(InferContext context) =>
            this.InferredType = context.ResolveType(this.InferredType);

        public override string ToString() =>
            $"{Utilities.PrettyPrint(this.Value)}:{this.InferredType}";
    }
}
