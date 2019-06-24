using Favalon.Expressions.Internals;

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

        public override bool ShowInAnnotation =>
            this.Parameter.ShowInAnnotation && this.Expression.ShowInAnnotation;

        protected override string FormatReadableString(bool withAnnotation) =>
            (this.Parameter is LambdaExpression) ?
                $"({this.Parameter.GetReadableString(withAnnotation)}) -> {this.Expression.GetReadableString(withAnnotation)}" :
                $"{this.Parameter.GetReadableString(withAnnotation)} -> {this.Expression.GetReadableString(withAnnotation)}";

        protected internal override Expression Visit(Environment environment, InferContext context)
        {
            var scoped = environment.NewScope();

            // Force replacing with new placeholder at the variable parameter.
            // Because the bind expression excepts inferring from derived environments,
            // but uses variable expression instead simple name string.
            // It requires annotation processing.
            var parameter = (this.Parameter is VariableExpression variable) ?
                variable.CreateWithPlaceholder(scoped, context) :
                this.Parameter.Visit(scoped, context);
            var expression = this.Expression.Visit(scoped, context);

            return new LambdaExpression(parameter, expression);
        }

        protected internal override TraverseResults Traverse(System.Func<Expression, int, Expression> yc, int rank)
        {
            this.Parameter = yc(this.Parameter, rank);
            this.Expression = yc(this.Expression, rank);

            return TraverseResults.RequeireHigherOrder;
        }
    }
}
