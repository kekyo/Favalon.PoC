using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Typed
{
    public abstract class TypedExpression : Expression
    {
        private protected TypedExpression(Type type, TextRegion textRegion) : base(textRegion) =>
            this.Type = type;

        public Type Type { get; private protected set; }

        public override bool IsResolved =>
            this.Type.IsResolved;

        internal abstract void Resolve(InferContext context);
    }
}
