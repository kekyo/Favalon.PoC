using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Internals
{
    internal sealed class PlaceholderController
    {
        private int index = 1;
        private readonly Dictionary<PlaceholderExpression, Expression> memoizedExpressions =
            new Dictionary<PlaceholderExpression, Expression>();

        public PlaceholderController()
        { }

        public PlaceholderExpression Create(Expression higherOrder) =>
            new PlaceholderExpression(index++, higherOrder);

        public void Memoize(PlaceholderExpression placeholder, Expression expression) =>
            memoizedExpressions.Add(placeholder, expression);

        public Expression? Lookup(PlaceholderExpression placeholder) =>
            memoizedExpressions.TryGetValue(placeholder, out var expression) ? expression : null;
    }
}
