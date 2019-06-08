using BasicSyntaxTree.Typed.Expressions;
using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Untyped.Expressions
{
    public sealed class UntypedLambdaExpression : UntypedExpression
    {
        public readonly string Parameter;
        public readonly UntypedExpression Expression;

        internal UntypedLambdaExpression(string parameter, UntypedExpression expression, TextRegion textRegion) : base(textRegion)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        internal override TypedExpression Visit(TypeEnvironment environment, InferContext context)
        {
            var scopedEnvironment = environment.MakeScope();
            var parameterType = context.CreateUntypedType();
            scopedEnvironment.RegisterVariable(this.Parameter, parameterType);
            var expression = this.Expression.Visit(scopedEnvironment, context);
            var type = Type.Function(parameterType, expression.Type);
            return new LambdaExpression(this.Parameter, expression, type, this.TextRegion);
        }

        public override string ToString() =>
            $"fun {this.Parameter} = {this.Expression}";
    }
}
