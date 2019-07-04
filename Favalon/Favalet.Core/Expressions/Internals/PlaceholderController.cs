using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Internals
{
    internal sealed class PlaceholderController
    {
        private int index = 1;
        private readonly Dictionary<Expression, Expression> memoizedExpressions =
            new Dictionary<Expression, Expression>();

        public PlaceholderController()
        { }

        public PlaceholderExpression Create(Expression higherOrder) =>
            new PlaceholderExpression(index++, higherOrder);

        public void Memoize(Expression symbol, Expression expression) =>
            memoizedExpressions.Add(symbol, expression);

        public Expression? Lookup(Expression symbol) =>
            memoizedExpressions.TryGetValue(symbol, out var expression) ? expression : null;
    }
}
