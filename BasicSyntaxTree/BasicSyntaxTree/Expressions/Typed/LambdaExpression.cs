using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Typed
{
    public sealed class LambdaExpression : TypedExpression
    {
        public readonly string Parameter;
        public readonly TypedExpression Body;

        internal LambdaExpression(string parameter, TypedExpression body, Type type, TextRegion textRegion) : base(type, textRegion)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        internal override void Resolve(InferContext context)
        {
            this.Body.Resolve(context);
            base.Resolve(context);
        }

        public override string ToString() =>
            $"{this.Parameter}:{this.Type} = {this.Body}";
    }
}
