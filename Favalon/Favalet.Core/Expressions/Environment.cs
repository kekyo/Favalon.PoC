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

        public Expression Infer(Expression expression, Expression higherOrderHint) =>
            Expression.VisitInferring(this, expression, higherOrderHint);
        public Expression Infer(Expression expression) =>
            Expression.VisitInferring(this, expression, UndefinedExpression.Instance);

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
