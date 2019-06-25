using System.Collections.Generic;

namespace Favalon.Expressions.Internals
{
    public sealed class InferContext
    {
        private readonly Dictionary<IdentityExpression, Expression> identities =
            new Dictionary<IdentityExpression, Expression>();

        internal InferContext() { }

        public void UnifyExpression(Expression expression1, Expression expression2)
        {
            // Pair of placehoders / one of placeholder
            if (expression1 is PlaceholderExpression placeholder1)
            {
                if (expression2 is PlaceholderExpression placeholder2)
                {
                    // Unify placeholder2 into placehoder1 if aren't same.
                    if (!placeholder1.Equals(placeholder2))
                    {
                        identities[placeholder1] = placeholder2;
                    }
                    return;
                }

                // Fallback placeholder1 below.
            }
            else if (expression2 is PlaceholderExpression)
            {
                // Swap primary expression and re-examine it.
                this.UnifyExpression(expression2, expression1);
                return;
            }

            // Pair of identities / one of identity (Include fallbacked placeholder)
            if (expression1 is IdentityExpression identity1)
            {
                if (expression2 is IdentityExpression identity2)
                {
                    // Unify identity2 into identity1 if aren't same.
                    if (!identity1.Equals(identity2))
                    {
                        identities[identity1] = expression2;
                    }
                }
                else if (identities.TryGetValue(identity1, out var resolved))
                {
                    this.UnifyExpression(expression2, resolved);
                }
                else
                {
                    // Unify expression2 into identity1.
                    identities[identity1] = expression2;
                }
                return;
            }
            else if (expression2 is IdentityExpression identity2)
            {
                if (identities.TryGetValue(identity2, out var resolved))
                {
                    this.UnifyExpression(expression1, resolved);
                }
                else
                {
                    // Unify expression1 into identity2.
                    identities[identity2] = expression1;
                }
                return;
            }

            // Unify lambdas each parameters and expressions.
            if ((expression1 is LambdaExpression lambda1) &&
                (expression2 is LambdaExpression lambda2))
            {
                this.UnifyExpression(lambda1.Parameter, lambda2.Parameter);
                this.UnifyExpression(lambda1.Expression, lambda2.Expression);
                return;
            }
        }

        public Expression FixupHigherOrders(Expression expression, int rank)
        {
            var current = expression;

            if (current is IdentityExpression identity)
            {
                if (identities.TryGetValue(identity, out var resolved))
                {
                    current = resolved;
                }
            }

            if (current.Traverse(this.FixupHigherOrders, rank) == Expression.TraverseResults.RequeireHigherOrder)
            {
                current.HigherOrder = this.FixupHigherOrders(current.HigherOrder, rank + 1);
            }

            return current;
        }

        public Expression AggregatePlaceholders(Expression expression, int rank)
        {
            if (expression.Traverse(this.AggregatePlaceholders, rank) == Expression.TraverseResults.RequeireHigherOrder)
            {
                expression.HigherOrder = this.AggregatePlaceholders(expression.HigherOrder, rank + 1);
            }

            return expression;
        }
    }
}
