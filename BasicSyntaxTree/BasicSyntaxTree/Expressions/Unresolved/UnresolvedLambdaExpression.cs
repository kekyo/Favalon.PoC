using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Unresolved
{
    public sealed class UnresolvedLambdaExpression : UnresolvedExpression
    {
        public readonly UnresolvedVariableExpression Parameter;
        public readonly UnresolvedExpression Expression;

        internal UnresolvedLambdaExpression(
            UnresolvedVariableExpression parameter, UnresolvedExpression expression, Type? annotatedType, TextRegion textRegion) :
            base(annotatedType, textRegion)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        internal override bool IsSafePrintable => false;

        internal override ResolvedExpression Visit(Environment environment, InferContext context)
        {
            var scopedEnvironment = environment.MakeScope();

            var parameter = (VariableExpression)this.Parameter.Visit(scopedEnvironment, context);
            var expression = this.Expression.Visit(scopedEnvironment, context);
            var type = Type.Function(parameter.InferredType, expression.InferredType);

            return new LambdaExpression(parameter, expression, type, this.TextRegion);
        }

        public override string ToString() =>
            $"fun {this.Parameter} -> {this.Expression}";
    }
}
