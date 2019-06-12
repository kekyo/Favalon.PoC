using BasicSyntaxTree.Untyped;

namespace BasicSyntaxTree.Expressions
{
    public sealed class BindExpression : TypedExpression
    {
        public readonly VariableExpression Target;
        public readonly TypedExpression Expression;
        public readonly TypedExpression Body;

        internal BindExpression(VariableExpression target, TypedExpression expression, TypedExpression body, TextRegion textRegion) : base(body.Type, textRegion)
        {
            this.Target = target;
            this.Expression = expression;
            this.Body = body;
        }

        internal override bool IsSafePrintable => false;

        internal override void Resolve(InferContext context)
        {
            this.Expression.Resolve(context);
            this.Body.Resolve(context);
            this.Target.Resolve(context);
            this.Type = context.ResolveType(this.Type);
        }

        public override string ToString() =>
            $"let {this.Target} = {this.Expression} in {this.Body}";
    }
}
