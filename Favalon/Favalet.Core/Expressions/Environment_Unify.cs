using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    using static Favalet.Expressions.Expression;

    partial class Environment
    {
        private Expression UnifyLambda(LambdaExpression lambda1, Expression expression2)
        {
            var parameter = this.Unify(lambda1.Parameter, UnspecifiedExpression.Instance);
            var expression = this.Unify(lambda1.Expression, UnspecifiedExpression.Instance);

            var lambdaHigherOrder = new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, UnspecifiedExpression.Instance);
            var lambda = new LambdaExpression(parameter, expression, lambdaHigherOrder);

            if (expression2 is PlaceholderExpression placeholder)
            {
                placehoderController.Memoize(placeholder, lambda);
            }
            else
            {
                Debug.Assert(!(expression2 is LambdaExpression));
                throw new ArgumentException($"Cannot unifying: between \"{lambda1.ReadableString}\" and \"{expression2.ReadableString}\"");
            }
            return lambda;
        }

        private Expression Unify2(Expression expression1, Expression expression2)
        {
            if (expression1.Equals(expression2))
            {
                return expression1;
            }

            if (expression1 is PlaceholderExpression placeholder12)
            {
                if (placehoderController.Lookup(placeholder12) is Expression lookup)
                {
                    return lookup;
                    //return this.Unify(lookup, expression2);
                }
            }
            if (expression2 is PlaceholderExpression placeholder22)
            {
                if (placehoderController.Lookup(placeholder22) is Expression lookup)
                {
                    return lookup;
                    //return this.Unify(lookup, expression1);
                }
            }

            if (expression2 is LambdaExpression lambda2)
            {
                if (expression1 is LambdaExpression lambda11)
                {
                    var parameter = this.Unify(lambda11.Parameter, lambda2.Parameter);
                    var expression = this.Unify(lambda11.Expression, lambda2.Expression);
                    var lambdaHigherOrder = new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, UnspecifiedExpression.Instance);

                    return new LambdaExpression(parameter, expression, lambdaHigherOrder);
                }
                else
                {
                    return this.UnifyLambda(lambda2, expression1);
                }
            }
            else if (expression1 is LambdaExpression lambda12)
            {
                return this.UnifyLambda(lambda12, expression2);
            }

            if (expression1 is PlaceholderExpression placeholder13)
            {
                placehoderController.Memoize(placeholder13, expression2);
                return expression2;
            }
            if (expression2 is PlaceholderExpression placeholder23)
            {
                placehoderController.Memoize(placeholder23, expression1);
                return expression1;
            }

            throw new ArgumentException($"Cannot unifying: between \"{expression1.ReadableString}\" and \"{expression2.ReadableString}\"");
        }

        internal Expression Unify(Expression expression1, Expression expression2)
        {
            if (expression2 is UnspecifiedExpression)
            {
                if (expression1 is UnspecifiedExpression)
                {
                    return this.CreatePlaceholder(UnspecifiedExpression.Instance);
                }
                else
                {
                    return expression1;
                }
            }
            else if (expression1 is UnspecifiedExpression)
            {
                return expression2;
            }

            return this.Unify2(expression1, expression2);
        }

        Expression IInferringEnvironment.Unify(Expression expression1, Expression expression2) =>
            this.Unify(expression1, expression2);

        Expression IInferringEnvironment.Unify(Expression expression1, Expression expression2, Expression expression3)
        {
            if (expression3 is UnspecifiedExpression)
            {
                if (expression2 is UnspecifiedExpression)
                {
                    if (expression1 is UnspecifiedExpression)
                    {
                        return this.CreatePlaceholder(UnspecifiedExpression.Instance);
                    }
                    else
                    {
                        return expression1;
                    }
                }
                else if (expression1 is UnspecifiedExpression)
                {
                    return expression2;
                }
                else
                {
                    return this.Unify2(expression1, expression2);
                }
            }
            else if (expression2 is UnspecifiedExpression)
            {
                if (expression1 is UnspecifiedExpression)
                {
                    return expression3;
                }
                else
                {
                    return this.Unify2(expression1, expression3);
                }
            }
            else if (expression1 is UnspecifiedExpression)
            {
                return this.Unify2(expression2, expression3);
            }
            else
            {
                return this.Unify2(expression1, this.Unify2(expression2, expression3));
            }
        }
    }
}
