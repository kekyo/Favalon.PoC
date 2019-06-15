using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions
{
    public sealed class KindTypeExpression : ResolvedExpression
    {
        internal KindTypeExpression(KindType kindType, TextRegion textRegion) :
            base(kindType, textRegion) { }

        internal override bool IsSafePrintable => false;

        internal override void Resolve(InferContext context) =>
            this.InferredType = context.ResolveType(this.InferredType);

        public override string ToString() =>
            this.InferredType.ToString();
    }
}
