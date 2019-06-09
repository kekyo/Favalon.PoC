using BasicSyntaxTree.Typed;
using BasicSyntaxTree.Typed.Expressions;

namespace BasicSyntaxTree.Untyped.Expressions
{
    public sealed class UntypedVariableExpression : UntypedExpression
    {
        public readonly string Name;

        internal UntypedVariableExpression(string name, TextRegion textRegion) : base(textRegion) =>
            this.Name = name;

        internal override TypedExpression Visit(TypeEnvironment environment, InferContext context) =>
            new VariableExpression(this.Name, environment.GetType(this.Name) ?? context.CreateUnspecifiedType(), this.TextRegion);

        public override string ToString() =>
            this.Name;
    }
}
