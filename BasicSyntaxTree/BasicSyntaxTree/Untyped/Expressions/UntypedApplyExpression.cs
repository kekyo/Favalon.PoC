using BasicSyntaxTree.Typed;
using BasicSyntaxTree.Typed.Expressions;
using BasicSyntaxTree.Typed.Types;

namespace BasicSyntaxTree.Untyped.Expressions
{
    public sealed class UntypedApplyExpression : UntypedExpression
    {
        public readonly UntypedExpression Function;
        public readonly UntypedExpression Argument;

        internal UntypedApplyExpression(
            UntypedExpression function, UntypedExpression argument, UntypedType? annotatedType,
            TextRegion textRegion) : base(annotatedType, textRegion)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal override bool IsSafePrintable => false;

        internal override TypedExpression Visit(Environment environment, InferContext context)
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
