using BasicSyntaxTree.Typed.Expressions;
using BasicSyntaxTree.Types;

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
            var returnType = context.CreateUntypedType();

            Unify(functionExpression.Type, Type.Function(argumentExpression.Type, returnType), context);

            return new ApplyExpression(functionExpression, argumentExpression, returnType, this.TextRegion);
        }

        public override string ToString() =>
            $"{this.Lambda} {this.Argument}";
    }
}
