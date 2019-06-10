using BasicSyntaxTree.Typed;
using BasicSyntaxTree.Typed.Expressions;

namespace BasicSyntaxTree.Untyped.Expressions
{
    public sealed class UntypedBindExpression : UntypedExpression
    {
        // let a = 123 in ...
        // let f = fun x -> (+) x 1 in ...
        public readonly UntypedVariableExpression Target;
        public readonly UntypedExpression Expression;
        public readonly UntypedExpression Body;

        internal UntypedBindExpression(
            UntypedVariableExpression target, UntypedExpression expression, UntypedExpression body, UntypedType? annotatedType,
            TextRegion textRegion) : base(annotatedType, textRegion)
        {
            this.Target = target;
            this.Expression = expression;
            this.Body = body;
        }

        internal override bool IsSafePrintable => false;

        internal override TypedExpression Visit(Environment environment, InferContext context)
        {
            var scopedEnvironment = environment.MakeScope();

            var target = (VariableExpression)this.Target.Visit(scopedEnvironment, context);
            var expression = this.Expression.Visit(scopedEnvironment, context);
            var body = this.Body.Visit(scopedEnvironment, context);

            context.Unify(expression.Type, body.Type);

            return new BindExpression(target, expression, body, this.TextRegion);
        }

        public override string ToString() =>
            $"let {this.Target} = {this.Expression} in {this.Body}";
    }
}
