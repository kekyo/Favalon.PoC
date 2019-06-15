using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions
{
    public sealed class LambdaExpression : ResolvedExpression
    {
        public readonly VariableExpression Parameter;
        public readonly ResolvedExpression Body;

        internal LambdaExpression(VariableExpression parameter, ResolvedExpression body, Type inferredType, TextRegion textRegion) :
            base(inferredType, textRegion)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        internal override bool IsSafePrintable => false;

        internal override void Resolve(InferContext context)
        {
            this.Body.Resolve(context);
            this.Parameter.Resolve(context);
            this.InferredType = context.ResolveType(this.InferredType);
        }

        public override string ToString() =>
            $"fun {this.Parameter.SafePrintable}:{this.InferredType} -> {this.Body}";
    }
}
