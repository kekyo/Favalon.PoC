namespace BasicSyntaxTree.Expressions.Typed
{
    public sealed class BindExpression : TypedExpression
    {
        public readonly string Name;
        public readonly TypedExpression Expression;
        public readonly TypedExpression Body;

        internal BindExpression(string name, TypedExpression expression, TypedExpression body, TextRegion textRegion) : base(body.Type, textRegion)
        {
            this.Name = name;
            this.Expression = expression;
            this.Body = body;
        }

        internal override void Resolve(InferContext context)
        {
            this.Expression.Resolve(context);
            this.Body.Resolve(context);
            this.Type = context.ResolveType(this.Type);
        }

        public override string ToString() =>
            $"let {this.Name}:{this.Expression.Type} = {this.Expression} in {this.Body}";
    }
}
