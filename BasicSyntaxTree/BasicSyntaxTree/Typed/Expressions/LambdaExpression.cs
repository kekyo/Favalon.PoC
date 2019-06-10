using BasicSyntaxTree.Untyped;

namespace BasicSyntaxTree.Typed.Expressions
{
    public sealed class LambdaExpression : TypedExpression
    {
        public readonly VariableExpression Parameter;
        public readonly TypedExpression Body;

        internal LambdaExpression(VariableExpression parameter, TypedExpression body, Type type, TextRegion textRegion) : base(type, textRegion)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        internal override bool IsSafePrintable => false;

        internal override void Resolve(InferContext context)
        {
            this.Body.Resolve(context);
            this.Parameter.Resolve(context);
            this.Type = context.ResolveType(this.Type);
        }


        public override string ToString() =>
            $"fun {this.Parameter.SafePrintable}:{this.Type} -> {this.Body}";
    }
}
