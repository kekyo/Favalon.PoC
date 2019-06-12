using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Unresolved
{
    public sealed class UnresolvedApplyExpression : UnresolvedExpression
    {
        public readonly UnresolvedExpression Function;
        public readonly UnresolvedExpression Argument;

        internal UnresolvedApplyExpression(
            UnresolvedExpression function, UnresolvedExpression argument, UnresolvedType? annotatedType,
            TextRegion textRegion) : base(annotatedType, textRegion)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal override bool IsSafePrintable => false;

        internal override ResolvedExpression Visit(Environment environment, InferContext context)
        {
            var functionExpression = this.Function.Visit(environment, context);
            var argumentExpression = this.Argument.Visit(environment, context);
            var thisExpressionType = this.AnnotetedType ?? context.CreateUnspecifiedType();

            context.Unify(functionExpression.Type, new FunctionType(argumentExpression.Type, thisExpressionType));

            return new ApplyExpression(functionExpression, argumentExpression, thisExpressionType, this.TextRegion);
        }

        public override string ToString() =>
            $"{this.Function} {this.Argument.SafePrintable}";
    }
}
