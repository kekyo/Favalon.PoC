using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class Environment
    {
        private sealed class PlaceholderController
        {
            private int index;

            public PlaceholderController()
            { }

            public PlaceholderExpression Create(Expression higherOrder) =>
                new PlaceholderExpression(index++, higherOrder);
        }

        private readonly PlaceholderController placehoderController = new PlaceholderController();

        private Environment()
        { }

        public PlaceholderExpression CreatePlaceholder(Expression higherOrder) =>
            placehoderController.Create(higherOrder);

        public Expression Infer(Expression expression)
        {
            return expression;
        }

        public TExpression Infer<TExpression>(TExpression expression)
            where TExpression : Expression =>
            (TExpression)this.Infer((Expression)expression);

        public static Environment Create() =>
            new Environment();
    }
}
