using BasicSyntaxTree.Untyped;
using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree.Typed.Expressions
{
    public sealed class LambdaExpression : TypedExpression
    {
        public readonly string Parameter;
        public readonly TypedExpression Body;

        internal LambdaExpression(string parameter, TypedExpression body, UntypedType type, TextRegion textRegion) : base(type, textRegion)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        internal override void Resolve(InferContext context)
        {
            this.Body.Resolve(context);
            this.Type = context.ResolveType(this.Type);
        }


        public override string ToString() =>
            $"fun {this.Parameter}:{this.Type} -> {this.Body}";
    }
}
