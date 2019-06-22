using System.Collections.Generic;
using System.Linq;

namespace Favalon.Expressions.Internals
{
    public sealed class InferContext
    {
        private readonly Dictionary<PlaceholderExpression, Expression> placeholders =
            new Dictionary<PlaceholderExpression, Expression>();
        private readonly SortedSet<PlaceholderExpression> collection =
            new SortedSet<PlaceholderExpression>();
        private int index;

        internal InferContext() { }

        public PlaceholderExpression CreatePlaceholder(int rank) =>
            new PlaceholderExpression(rank, index++);

        public void UnifyExpression(Expression expression1, Expression expression2)
        {
            if (expression1 is PlaceholderExpression placeholder1)
            {
                if (expression2 is PlaceholderExpression placeholder2)
                {
                    if (!placeholder1.Equals(placeholder2))
                    {
                        placeholders[placeholder1] = expression2;
                    }
                    return;
                }
                else if (placeholders.TryGetValue(placeholder1, out var resolved))
                {
                    this.UnifyExpression(expression2, resolved);
                }
                else
                {
                    placeholders[placeholder1] = expression2;
                }
            }
            else if (expression2 is PlaceholderExpression placeholder2)
            {
                if (placeholders.TryGetValue(placeholder2, out var resolved))
                {
                    this.UnifyExpression(expression1, resolved);
                }
                else
                {
                    placeholders[placeholder2] = expression1;
                }
            }
        }

        public Expression FixupHigherOrders(Expression expression)
        {
            if (expression is PlaceholderExpression placeholder)
            {
                if (placeholders.TryGetValue(placeholder, out var resolved))
                {
                    return resolved;
                }
            }

            if (expression.TraverseChildren(this.FixupHigherOrders))
            {
                expression.HigherOrder = this.FixupHigherOrders(expression.HigherOrder);
            }

            return expression;
        }

        public Expression AggregatePlaceholders(Expression expression)
        {
            if (expression is PlaceholderExpression placeholder)
            {
                collection.Add(placeholder);
            }

            if (expression.TraverseChildren(this.AggregatePlaceholders))
            {
                expression.HigherOrder = this.AggregatePlaceholders(expression.HigherOrder);
            }

            return expression;
        }

        internal void FixupPlaceholders()
        {
            foreach (var entry in collection.Select((ph, index) => (ph, index)))
            {
                entry.ph.Index = entry.index;
            }
        }
    }
}
