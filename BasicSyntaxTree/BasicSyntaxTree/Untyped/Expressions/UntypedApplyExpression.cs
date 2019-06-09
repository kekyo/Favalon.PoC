using BasicSyntaxTree.Typed;
using BasicSyntaxTree.Typed.Expressions;
using BasicSyntaxTree.Typed.Types;

namespace BasicSyntaxTree.Untyped.Expressions
{
    public sealed class UntypedApplyExpression : UntypedExpression
    {
        public new readonly UntypedExpression Lambda;
        public readonly UntypedExpression Argument;

        internal UntypedApplyExpression(UntypedExpression lambda, UntypedExpression argument, TextRegion textRegion) : base(textRegion)
        {
            this.Lambda = lambda;
            this.Argument = argument;
        }

        internal override TypedExpression Visit(TypeEnvironment environment, InferContext context)
        {
            var functionExpression = this.Lambda.Visit(environment, context);
            var argumentExpression = this.Argument.Visit(environment, context);
            var returnType = context.CreateUnspecifiedType();

            context.Unify(functionExpression.Type, new FunctionType(argumentExpression.Type, returnType));

            return new ApplyExpression(functionExpression, argumentExpression, returnType, this.TextRegion);
        }

        public override string ToString() =>
            $"{this.Lambda} {this.Argument}";
    }
}
