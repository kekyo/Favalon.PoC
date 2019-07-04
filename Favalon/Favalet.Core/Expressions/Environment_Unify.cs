using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    partial class Environment
    {
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

            if (expression1.Equals(expression2))
            {
                return expression1;
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
                    var parameter = this.Unify(lambda2.Parameter, UndefinedExpression.Instance);
                    var expression = this.Unify(lambda2.Expression, UndefinedExpression.Instance);

                    var lambdaHigherOrder = new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, UndefinedExpression.Instance);
                    var lambda = new LambdaExpression(parameter, expression, lambdaHigherOrder);

                    if (expression1 is PlaceholderExpression placeholder11)
                    {
                        placehoderController.Memoize(placeholder11, lambda);
                        return lambda;
                    }
                    else
                    {
                        return lambda;
                    }
                }
            }
            else if (expression1 is LambdaExpression lambda12)
            {
                var parameter = this.Unify(lambda12.Parameter, UndefinedExpression.Instance);
                var expression = this.Unify(lambda12.Expression, UndefinedExpression.Instance);

                var lambdaHigherOrder = new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, UndefinedExpression.Instance);
                var lambda = new LambdaExpression(parameter, expression, lambdaHigherOrder);

                if (expression2 is PlaceholderExpression placeholder21)
                {
                    placehoderController.Memoize(placeholder21, lambda);
                    return lambda;
                }
                else
                {
                    return lambda;
                }
            }

            if (expression1 is PlaceholderExpression placeholder12)
            {
                if (placehoderController.Lookup(placeholder12) is Expression lookup)
                {
                    return lookup;
                    //return this.Unify(lookup, expression2);
                }
                else
                {
                    placehoderController.Memoize(placeholder12, expression2);
                    return expression2;
                }
            }
            if (expression2 is PlaceholderExpression placeholder22)
            {
                if (placehoderController.Lookup(placeholder22) is Expression lookup)
                {
                    return lookup;
                    //return this.Unify(lookup, expression1);
                }
                else
                {
                    placehoderController.Memoize(placeholder22, expression1);
                    return expression1;
                }
            }

            throw new ArgumentException($"Cannot unifying: between \"{expression1.ReadableString}\" and \"{expression2.ReadableString}\"");
        }
    }
}
