using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Unresolved
{
    public sealed class UnresolvedBindExpression : UnresolvedExpression
    {
        // let a = 123 in ...
        // let f = fun x -> (+) x 1 in ...
        public readonly UnresolvedVariableExpression Target;
        public readonly UnresolvedExpression Expression;
        public readonly UnresolvedExpression Body;

        internal UnresolvedBindExpression(
            UnresolvedVariableExpression target, UnresolvedExpression expression, UnresolvedExpression body, UnresolvedType? annotatedType,
            TextRegion textRegion) : base(annotatedType, textRegion)
        {
            this.Target = target;
            this.Expression = expression;
            this.Body = body;
        }

        internal override bool IsSafePrintable => false;

        internal override ResolvedExpression Visit(Environment environment, InferContext context)
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
