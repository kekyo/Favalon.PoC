using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions
{
    public sealed class ApplyExpression : ResolvedExpression
    {
        public readonly ResolvedExpression Function;
        public readonly ResolvedExpression Argument;

        internal ApplyExpression(ResolvedExpression function, ResolvedExpression argument, Type inferredType, TextRegion textRegion) :
            base(inferredType, textRegion)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal override bool IsSafePrintable => false;

        internal override void Resolve(InferContext context)
        {
            this.Function.Resolve(context);
            this.Argument.Resolve(context);
            this.InferredType = context.ResolveType(this.InferredType);
        }

        public override string ToString() =>
            $"({this.Function} {this.Argument.SafePrintable}):{this.InferredType}";
    }
}
