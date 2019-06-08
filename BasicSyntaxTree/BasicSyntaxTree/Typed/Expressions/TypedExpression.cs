using BasicSyntaxTree.Untyped;
using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree.Typed.Expressions
{
    public abstract class TypedExpression : Expression
    {
        private protected TypedExpression(UntypedType type, TextRegion textRegion) : base(textRegion) =>
            this.Type = type;

        public UntypedType Type { get; private protected set; }

        public override bool IsResolved =>
            this.Type.IsResolved;

        internal abstract void Resolve(InferContext context);
    }
}
