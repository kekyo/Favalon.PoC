namespace Favalet.Expressions
{
    partial class Expression
    {
        protected internal interface IInferringEnvironment
        {
            Expression Unify(Expression expression1, Expression expression2);
            Expression Unify(Expression expression1, Expression expression2, Expression expression3);

            void Memoize(VariableExpression symbol, Expression expression);

            Expression? Lookup(VariableExpression symbol);

            TExpression Visit<TExpression>(TExpression expression, Expression higherOrderHint)
                where TExpression : Expression;
        }

        protected internal interface IResolvingEnvironment
        {
            Expression? Lookup(VariableExpression symbol);

            TExpression Visit<TExpression>(TExpression expression)
                where TExpression : Expression;
        }

        internal Expression InternalVisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            this.VisitInferring(environment, higherOrderHint);
        internal Expression InternalVisitResolving(IResolvingEnvironment environment) =>
            this.VisitResolving(environment);
    }
}
