using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions
{
    public abstract class ResolvedExpression : Expression
    {
        private protected ResolvedExpression(Type inferredType, TextRegion textRegion) : base(textRegion) =>
            this.InferredType = inferredType;

        public Type InferredType { get; private protected set; }

        public override bool IsResolved =>
            this.InferredType.IsResolved;

        internal abstract void Resolve(InferContext context);
    }
}
