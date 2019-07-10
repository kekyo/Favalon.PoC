using Favalet.Expressions.Specialized;
using System;
using System.Diagnostics;

namespace Favalet.Expressions
{
    using static Favalet.Expressions.Expression;

    partial class Environment
    {
        private Expression UnifyLambda(LambdaExpression lambda1, Expression expression2)
        {
            var unspecified2 = (lambda1.Parameter.HigherOrder, lambda1.Expression.HigherOrder, expression2.HigherOrder) switch {
                (_, _, KindExpression _) => UnspecifiedExpression.Kind,
                (KindExpression _, _, _) => UnspecifiedExpression.Kind,
                (_, KindExpression _, _) => UnspecifiedExpression.Kind,
                (_, _, TypeExpression _) => UnspecifiedExpression.Type,
                (TypeExpression _, _, _) => UnspecifiedExpression.Type,
                (_, TypeExpression _, _) => UnspecifiedExpression.Type,
                _ => UnspecifiedExpression.Instance
            };

            var parameter = this.Unify(lambda1.Parameter, unspecified2);
            var expression = this.Unify(lambda1.Expression, unspecified2);

            var lambdaHigherOrder = new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, UnspecifiedExpression.Instance);
            var lambda = new LambdaExpression(parameter, expression, lambdaHigherOrder);

            if (expression2 is PlaceholderExpression placeholder)
            {
                placehoderController.Memoize(placeholder, lambda);
            }
            else if (!(expression2 is KindExpression || expression2 is TypeExpression))
            {
                Debug.Assert(!(expression2 is LambdaExpression));
                throw new ArgumentException($"Cannot unify: between \"{lambda1.StrictReadableString}\" and \"{expression2.StrictReadableString}\"");
            }

            return lambda;
        }

        private Expression Unify2(Expression expression1, Expression expression2)
        {
            Debug.Assert(
                !(expression1 is UnspecifiedExpression) &&
                !(expression2 is UnspecifiedExpression));

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

            throw new ArgumentException($"Cannot unify: between \"{expression1.ReadableString}\" and \"{expression2.ReadableString}\"");
        }

        private Expression CreatePlaceholderIfRequired(Expression expression) =>
            expression switch
            {
                UnspecifiedExpression _ => this.CreatePlaceholder(UnspecifiedExpression.Instance),
                TypeExpression _ => this.CreatePlaceholder(KindExpression.Instance),
                _ when expression.HigherOrder is TypeExpression => this.CreatePlaceholder(TypeExpression.Instance),
                _ => expression
            };

        internal Expression Unify(Expression expression1, Expression expression2)
        {
            Expression result;

            if (expression2 is UnspecifiedExpression)
            {
                result = expression1;
            }
            else if (expression1 is UnspecifiedExpression)
            {
                result = expression2;
            }
            else
            {
                result = this.Unify2(expression1, expression2);
            }

            return this.CreatePlaceholderIfRequired(result);
        }

        Expression IInferringEnvironment.Unify(Expression expression1, Expression expression2) =>
            this.Unify(expression1, expression2);

        Expression IInferringEnvironment.Unify(Expression expression1, Expression expression2, Expression expression3)
        {
            Expression result;

            if (expression3 is UnspecifiedExpression)
            {
                if (expression2 is UnspecifiedExpression)
                {
                    result = expression1;
                }
                else if (expression1 is UnspecifiedExpression)
                {
                    result = expression2;
                }
                else
                {
                    result = this.Unify2(expression1, expression2);
                }
            }
            else if (expression2 is UnspecifiedExpression)
            {
                if (expression1 is UnspecifiedExpression)
                {
                    result = expression3;
                }
                else
                {
                    result = this.Unify2(expression1, expression3);
                }
            }
            else if (expression1 is UnspecifiedExpression)
            {
                result = this.Unify2(expression2, expression3);
            }
            else
            {
                result = this.Unify2(expression1, this.Unify2(expression2, expression3));
            }

            return this.CreatePlaceholderIfRequired(result);
        }
    }
}
