using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions
{
    public sealed class BindExpression : ResolvedExpression
    {
        public readonly VariableExpression Target;
        public readonly ResolvedExpression Expression;
        public readonly ResolvedExpression Body;

        internal BindExpression(VariableExpression target, ResolvedExpression expression, ResolvedExpression body, Type inferredType, TextRegion textRegion) :
            base(inferredType, textRegion)
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
            this.InferredType = context.ResolveType(this.InferredType);
        }

        public override string ToString() =>
            $"let {this.Target} = {this.Expression} in {this.Body}";
    }
}
