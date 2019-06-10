using BasicSyntaxTree.Typed;
using BasicSyntaxTree.Typed.Expressions;
using BasicSyntaxTree.Typed.Types;

namespace BasicSyntaxTree.Untyped.Expressions
{
    public sealed class UntypedLambdaExpression : UntypedExpression
    {
        public readonly UntypedVariableExpression Parameter;
        public readonly UntypedExpression Expression;

        internal UntypedLambdaExpression(
            UntypedVariableExpression parameter, UntypedExpression expression, UntypedType? annotatedType,
            TextRegion textRegion) : base(annotatedType, textRegion)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        internal override bool IsSafePrintable => false;

        internal override TypedExpression Visit(Environment environment, InferContext context)
        {
            var scopedEnvironment = environment.MakeScope();

            var parameter = (VariableExpression)this.Parameter.Visit(scopedEnvironment, context);
            var expression = this.Expression.Visit(scopedEnvironment, context);
            var type = new FunctionType(parameter.Type, expression.Type);

            return new LambdaExpression(parameter, expression, type, this.TextRegion);
        }

        public override string ToString() =>
            $"fun {this.Parameter} -> {this.Expression}";
    }
}
