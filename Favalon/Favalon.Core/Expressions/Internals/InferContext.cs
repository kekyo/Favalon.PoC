﻿using System.Collections.Generic;
using System.Linq;

namespace Favalon.Expressions.Internals
{
    public sealed class InferContext
    {
        private readonly Dictionary<IIdentityExpression, Expression> identities =
            new Dictionary<IIdentityExpression, Expression>();

        internal InferContext() { }

        public void UnifyExpression(Expression expression1, Expression expression2)
        {
            // Pair of placehoders / one of freeVariable
            if (expression1 is PlaceholderExpression freeVariable1)
            {
                if (expression2 is PlaceholderExpression freeVariable2)
                {
                    // Unify freeVariable2 into freeVariable1 if aren't same.
                    if (!freeVariable1.Equals(freeVariable2))
                    {
                        identities[freeVariable1] = freeVariable2;
                    }
                    return;
                }

                // Fallback freeVariable1 below.
            }
            else if (expression2 is PlaceholderExpression)
            {
                // Swap primary expression and re-examine it.
                this.UnifyExpression(expression2, expression1);
                return;
            }

            // Pair of identities / one of identity (Include fallbacked freeVariable)
            if (expression1 is IIdentityExpression identity1)
            {
                if (expression2 is IIdentityExpression identity2)
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
            else if (expression2 is IIdentityExpression identity2)
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
            if ((expression1 is ILambdaExpression lambda1) &&
                (expression2 is ILambdaExpression lambda2))
            {
                this.UnifyExpression(lambda1.Parameter, lambda2.Parameter);
                this.UnifyExpression(lambda1.Expression, lambda2.Expression);
                return;
            }
        }

        public TExpression FixupHigherOrders<TExpression>(TExpression expression, int rank)
            where TExpression : Expression
        {
            // Inferring final step.

            // Replace unified higher orders.
            Expression current = expression;
            if (current is IIdentityExpression identity)
            {
                if (identities.TryGetValue(identity, out var resolved))
                {
                    current = resolved;
                }
            }

            // Recursive check onto higher order expressions.
            if (current.FixupHigherOrders(this, rank) == Expression.TraverseInferringResults.RequeireHigherOrder)
            {
                current.SetHigherOrder(this.FixupHigherOrders(current.HigherOrder, rank + 1));
            }

            return (TExpression)current;
        }

        public override string ToString() =>
            string.Join(", ", identities.Select(entry => $"{entry.Key.ReadableString}={entry.Value.ReadableString}"));
    }
}
