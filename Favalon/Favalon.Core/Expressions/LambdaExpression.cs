namespace Favalon.Expressions
{
    public sealed class LambdaExpression : Expression
    {
        // 'a -> 'b
        public Expression Parameter { get; private set; }
        public Expression Expression { get; private set; }

        private LambdaExpression(Expression parameter, Expression expression, Expression higherOrder) :
            base(higherOrder)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        internal LambdaExpression(Expression parameter, Expression expression) :
            this(parameter, expression,
                new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, KindExpression.Instance))
        {
        }

        internal override bool CanProduceSafeReadableString =>
            false;
        internal override bool IsIgnoreReadableString =>
            this.Parameter.IsIgnoreReadableString || this.Expression.IsIgnoreReadableString;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            $"{this.Parameter.GetReadableString(withAnnotation)} -> {this.Expression.GetReadableString(withAnnotation)}";

        internal override Expression Visit(ExpressionEnvironment environment, InferContext context)
        {
            var scoped = environment.NewScope();

            var parameter = this.Parameter.Visit(scoped, context);
            var expression = this.Expression.Visit(scoped, context);

            return new LambdaExpression(parameter, expression);
        }

        internal override Expression FixupChildren(InferContext context)
        {
            this.Parameter = context.Fixup(this.Parameter);
            this.Expression = context.Fixup(this.Expression);
            this.HigherOrder = context.Fixup(this.HigherOrder);
            return this;
        }
    }
}
