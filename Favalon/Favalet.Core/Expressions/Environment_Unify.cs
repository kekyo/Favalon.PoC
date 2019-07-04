using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    partial class Environment
    {
        private Expression UnifyLambda(LambdaExpression lambda1, Expression expression2)
        {
            var parameter = this.Unify(lambda1.Parameter, UndefinedExpression.Instance);
            var expression = this.Unify(lambda1.Expression, UndefinedExpression.Instance);

            var lambdaHigherOrder = new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, UndefinedExpression.Instance);
            var lambda = new LambdaExpression(parameter, expression, lambdaHigherOrder);

            if (expression2 is PlaceholderExpression placeholder)
            {
                placehoderController.Memoize(placeholder, lambda);
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
                    var lambdaHigherOrder = new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, UndefinedExpression.Instance);

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
            if (expression2 is UndefinedExpression)
            {
                if (expression1 is UndefinedExpression)
                {
                    return this.CreatePlaceholder(UndefinedExpression.Instance);
                }
                else
                {
                    return expression1;
                }
            }
            else if (expression1 is UndefinedExpression)
            {
                return expression2;
            }

            return this.Unify2(expression1, expression2);
        }

        internal Expression Unify(Expression expression1, Expression expression2, Expression expression3)
        {
            if (expression3 is UndefinedExpression)
            {
                if (expression2 is UndefinedExpression)
                {
                    if (expression1 is UndefinedExpression)
                    {
                        return this.CreatePlaceholder(UndefinedExpression.Instance);
                    }
                    else
                    {
                        return expression1;
                    }
                }
                else if (expression1 is UndefinedExpression)
                {
                    return expression2;
                }
                else
                {
                    return this.Unify2(expression1, expression2);
                }
            }
            else if (expression2 is UndefinedExpression)
            {
                if (expression1 is UndefinedExpression)
                {
                    return expression3;
                }
                else
                {
                    return this.Unify2(expression1, expression3);
                }
            }
            else if (expression1 is UndefinedExpression)
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
