using System;

namespace Favalon.Expressions
{
    public sealed class LambdaExpression : Expression
    {
        // x -> expr
        public readonly VariableExpression Parameter;
        public readonly Expression Expression;

        private LambdaExpression(VariableExpression parameter, Expression expression, Expression higherOrder) :
            base(higherOrder)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        internal LambdaExpression(VariableExpression parameter, Expression expression) :
            this(parameter, expression, new FunctionExpression(parameter.HigherOrder, expression.HigherOrder))
        {
        }

        internal override bool CanProduceSafeReadableString =>
            false;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            $"{this.Parameter.GetReadableString(withAnnotation)} -> {this.Expression.GetReadableString(withAnnotation)}";

        internal override Expression Visit(ExpressionEnvironment environment)
        {
            var scoped = environment.NewScope();

            var parameter = (VariableExpression)this.Parameter.Visit(scoped);
            var expression = this.Expression.Visit(scoped);
            var resultHigherOrder = new FunctionExpression(parameter.HigherOrder, expression.HigherOrder);

            return new LambdaExpression(parameter, expression, resultHigherOrder);
        }

        internal override void Resolve(ExpressionEnvironment environment)
        {
            this.Parameter.Resolve(environment);
            this.Expression.Resolve(environment);
        }
    }
}
