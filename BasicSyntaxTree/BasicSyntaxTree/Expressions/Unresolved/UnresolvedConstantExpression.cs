using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Unresolved
{
    public sealed class UnresolvedConstantExpression : UnresolvedExpression
    {
        public readonly object Value;

        internal UnresolvedConstantExpression(object value, TextRegion textRegion) :
            base(Type.Runtime(value.GetType()), textRegion) =>
            this.Value = value;

        internal override bool IsSafePrintable => true;

        internal override ResolvedExpression Visit(Environment environment, InferContext context) =>
            new ConstantExpression(this.Value, this.AnnotetedType!, this.TextRegion);

        public override string ToString() =>
            Utilities.PrettyPrint(this.Value);
    }
}
