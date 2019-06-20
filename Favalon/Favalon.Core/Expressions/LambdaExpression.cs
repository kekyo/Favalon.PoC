using System;

namespace Favalon.Expressions
{
    public sealed class LambdaExpression : Expression
    {
        // 'a -> 'b
        public readonly Expression Parameter;
        public readonly Expression Expression;

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

        internal override Expression Visit(ExpressionEnvironment environment)
        {
            var parameter = this.Parameter.Visit(environment);
            var expression = this.Expression.Visit(environment);

            return new LambdaExpression(parameter, expression);
        }

        internal override void Resolve(ExpressionEnvironment environment)
        {
            this.Parameter.Resolve(environment);
            this.Expression.Resolve(environment);
            this.HigherOrder = environment.Resolve(this.HigherOrder);
        }
    }
}
