using BasicSyntaxTree.Untyped;

namespace BasicSyntaxTree.Expressions
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
