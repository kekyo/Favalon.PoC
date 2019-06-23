using System.Collections.Generic;
using System.Linq;

namespace Favalon.Expressions.Internals
{
    public sealed class InferContext
    {
        private readonly Dictionary<IdentityExpression, Expression> identities =
            new Dictionary<IdentityExpression, Expression>();
        private readonly SortedSet<PlaceholderExpression> collection =
            new SortedSet<PlaceholderExpression>();
        private int index;

        internal InferContext() { }

        public PlaceholderExpression CreatePlaceholder() =>
            new PlaceholderExpression(index++);

        public void UnifyExpression(Expression expression1, Expression expression2)
        {
            if (expression1 is IdentityExpression identity1)
            {
                if (expression2 is IdentityExpression identity2)
                {
                    if (!identity1.Equals(identity2))
                    {
                        identities[identity1] = expression2;
                    }
                    return;
                }
                else if (identities.TryGetValue(identity1, out var resolved))
                {
                    this.UnifyExpression(expression2, resolved);
                }
                else
                {
                    identities[identity1] = expression2;
                }
            }
            else if (expression2 is IdentityExpression identity2)
            {
                if (identities.TryGetValue(identity2, out var resolved))
                {
                    this.UnifyExpression(expression1, resolved);
                }
                else
                {
                    identities[identity2] = expression1;
                }
            }
        }

        public Expression FixupHigherOrders(Expression expression, int rank)
        {
            if (expression is IdentityExpression identity)
            {
                if (identities.TryGetValue(identity, out var resolved))
                {
                    return resolved;
                }
            }

            if (expression.TraverseChildren(this.FixupHigherOrders, rank))
            {
                expression.HigherOrder = this.FixupHigherOrders(expression.HigherOrder, rank + 1);
            }

            return expression;
        }

        public Expression AggregatePlaceholders(Expression expression, int rank)
        {
            if (expression is PlaceholderExpression placeholder)
            {
                collection.Add(placeholder);
            }

            if (expression.TraverseChildren(this.AggregatePlaceholders, rank))
            {
                expression.HigherOrder = this.AggregatePlaceholders(expression.HigherOrder, rank + 1);
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
