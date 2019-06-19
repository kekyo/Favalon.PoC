namespace Favalon.Expressions
{
    public sealed class BindExpression : Expression
    {
        public readonly VariableExpression Variable;
        public readonly Expression Expression;
        public readonly Expression Body;

        internal BindExpression(VariableExpression variable, Expression expression, Expression body) :
            base(body.HigherOrder)
        {
            this.Variable = variable;
            this.Expression = expression;
            this.Body = body;
        }

        internal override bool CanProduceSafeReadableString =>
            false;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            $"{this.Variable.GetReadableString(withAnnotation)} = {this.Expression.GetReadableString(withAnnotation)} in {this.Body.GetReadableString(withAnnotation)}";

        internal override Expression Visit(ExpressionEnvironment environment)
        {
            var scoped = environment.NewScope();

            var variable = (VariableExpression)this.Variable.Visit(scoped);
            var expression = this.Expression.Visit(scoped);

            scoped.SetHigherOrder(variable.Name, expression.HigherOrder);
            scoped.UnifyExpression(variable.HigherOrder, expression.HigherOrder);

            //variable.HigherOrder = expression.HigherOrder;

            var body = this.Body.Visit(scoped);

            return new BindExpression(variable, expression, body);
        }

        internal override void Resolve(ExpressionEnvironment environment)
        {
            this.Variable.Resolve(environment);
            this.Expression.Resolve(environment);
            this.Body.Resolve(environment);
        }
    }
}
