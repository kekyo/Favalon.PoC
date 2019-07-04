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
                if (expression1 is LambdaExpression lambda1)
                {
                    var parameter = this.Unify(lambda1.Parameter, lambda2.Parameter);
                    var expression = this.Unify(lambda1.Expression, lambda2.Expression);
                    var lambdaHigherOrder = new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, UndefinedExpression.Instance);

                    return new LambdaExpression(parameter, expression, lambdaHigherOrder);
                }
            }

            if (expression1 is PlaceholderExpression placeholder1)
            {
                if (placehoderController.Lookup(placeholder1) is Expression lookup)
                {
                    return lookup;
                    //return this.Unify(lookup, expression2);
                }
                else
                {
                    placehoderController.Memoize(placeholder1, expression2);
                    return expression2;
                }
            }
            if (expression2 is PlaceholderExpression placeholder2)
            {
                if (placehoderController.Lookup(placeholder2) is Expression lookup)
                {
                    return lookup;
                    //return this.Unify(lookup, expression1);
                }
                else
                {
                    placehoderController.Memoize(placeholder2, expression1);
                    return expression1;
                }
            }

            throw new ArgumentException($"Cannot unifying: between \"{expression1.ReadableString}\" and \"{expression2.ReadableString}\"");
        }
    }
}
