using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Typed
{
    public abstract class TypedExpression : Expression
    {
        private protected TypedExpression(Type type, TextRegion textRegion) : base(textRegion) =>
            this.Type = type;

        public Type Type { get; private set; }

        public override bool IsResolved =>
            this.Type.IsResolved;

        internal virtual void Resolve(InferContext context) =>
            this.Type = context.ResolveType(this.Type);
    }
}
