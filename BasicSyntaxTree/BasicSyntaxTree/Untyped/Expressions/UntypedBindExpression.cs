using BasicSyntaxTree.Typed.Expressions;

namespace BasicSyntaxTree.Untyped.Expressions
{
    public sealed class UntypedBindExpression : UntypedExpression
    {
        // let a = 123 in ...
        // let f = fun x -> (+) x 1 in ...
        public readonly string Name;
        public readonly UntypedExpression Expression;
        public readonly UntypedExpression Body;

        internal UntypedBindExpression(string name, UntypedExpression expression, UntypedExpression body, TextRegion textRegion) : base(textRegion)
        {
            this.Name = name;
            this.Expression = expression;
            this.Body = body;
        }

        internal override TypedExpression Visit(TypeEnvironment environment, InferContext context)
        {
            var scopedEnvironment = environment.MakeScope();
            var boundType = context.CreateUnspecifiedType();

            scopedEnvironment.RegisterVariable(this.Name, boundType);
            var expression = this.Expression.Visit(scopedEnvironment, context);
            var body = this.Body.Visit(scopedEnvironment, context);

            Unify(expression.Type, body.Type, context);

            return new BindExpression(this.Name, expression, body, this.TextRegion);
        }

        public override string ToString() =>
            $"let {this.Name} = {this.Expression} in {this.Body}";
    }
}
