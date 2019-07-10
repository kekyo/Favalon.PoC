using Favalet.Expressions.Internals;
using Favalet.Expressions.Specialized;

namespace Favalet.Expressions
{
    using static Favalet.Expressions.Expression;

    public sealed partial class Environment :
        IInferringEnvironment, IResolvingEnvironment
    {
        private readonly PlaceholderController placehoderController = new PlaceholderController();

        private Environment()
        { }

        public PlaceholderExpression CreatePlaceholder(Expression higherOrder) =>
            placehoderController.Create(higherOrder);

        Expression IInferringEnvironment.CreatePlaceholderIfRequired(Expression from) =>
            (from is UnspecifiedExpression) ? this.CreatePlaceholder(UnspecifiedExpression.Instance) : from;

        void IInferringEnvironment.Memoize(VariableExpression symbol, Expression expression) =>
            placehoderController.Memoize(symbol, expression);

        Expression? IInferringEnvironment.Lookup(VariableExpression symbol) =>
            placehoderController.Lookup(symbol);
        Expression? IResolvingEnvironment.Lookup(VariableExpression symbol) =>
            placehoderController.Lookup(symbol);

        TExpression IInferringEnvironment.Visit<TExpression>(TExpression expression, Expression higherOrderHint) =>
            (TExpression)expression.InternalVisitInferring(this, higherOrderHint);
        TExpression IResolvingEnvironment.Visit<TExpression>(TExpression expression) =>
            (TExpression)expression.InternalVisitResolving(this);

        public Expression Infer(Expression expression, Expression higherOrderHint)
        {
            var partial = expression.InternalVisitInferring(this, higherOrderHint);
            return partial.InternalVisitResolving(this);
        }
        public Expression Infer(Expression expression) =>
            this.Infer(expression, UnspecifiedExpression.Instance);

        public TExpression Infer<TExpression>(TExpression expression, Expression higherOrderHint)
            where TExpression : Expression =>
            (TExpression)this.Infer((Expression)expression, higherOrderHint);
        public TExpression Infer<TExpression>(TExpression expression)
            where TExpression : Expression =>
            (TExpression)this.Infer((Expression)expression, UnspecifiedExpression.Instance);

        public static Environment Create() =>
            new Environment();
    }
}
