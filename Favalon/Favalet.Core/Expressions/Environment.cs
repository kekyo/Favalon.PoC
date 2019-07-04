using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed partial class Environment
    {
        private readonly PlaceholderController placehoderController = new PlaceholderController();

        private Environment()
        { }

        internal PlaceholderExpression CreatePlaceholder(Expression higherOrder) =>
            placehoderController.Create(higherOrder);

        internal void Memoize(Expression symbol, Expression expression) =>
            placehoderController.Memoize(symbol, expression);

        internal Expression? Lookup(Expression symbol) =>
            placehoderController.Lookup(symbol);

        public Expression Infer(Expression expression, Expression higherOrderHint)
        {
            var partial = Expression.VisitInferring(this, expression, higherOrderHint);
            return Expression.VisitResolving(this, partial);
        }
        public Expression Infer(Expression expression) =>
            this.Infer(expression, UndefinedExpression.Instance);

        public TExpression Infer<TExpression>(TExpression expression, Expression higherOrderHint)
            where TExpression : Expression =>
            (TExpression)this.Infer((Expression)expression, higherOrderHint);
        public TExpression Infer<TExpression>(TExpression expression)
            where TExpression : Expression =>
            (TExpression)this.Infer((Expression)expression, UndefinedExpression.Instance);

        public static Environment Create() =>
            new Environment();
    }
}
