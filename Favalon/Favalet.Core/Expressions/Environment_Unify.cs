// This is part of Favalon project.
// Copyright (c) 2019 Kouji Matsui
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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

        private Expression UnifyPlaceholder(PlaceholderExpression placeholder, Expression expression)
        {
            var fixedExpression = (placeholder.HigherOrder, expression) switch
            {
                (UnspecifiedExpression _, KindExpression _) => placeholder.InternalCloneWithHigherOrder(null!),
                (UnspecifiedExpression _, TypeExpression _) => placeholder.InternalCloneWithHigherOrder(KindExpression.Instance),
                _ => expression
            };

            placehoderController.Memoize(placeholder, fixedExpression);
            return fixedExpression;
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
                }
            }
            if (expression2 is PlaceholderExpression placeholder22)
            {
                if (placehoderController.Lookup(placeholder22) is Expression lookup)
                {
                    return lookup;
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
                return this.UnifyPlaceholder(placeholder13, expression2);
            }
            if (expression2 is PlaceholderExpression placeholder23)
            {
                return this.UnifyPlaceholder(placeholder23, expression1);
            }

            throw new ArgumentException($"Cannot unify: between \"{expression1.ReadableString}\" and \"{expression2.ReadableString}\"");
        }

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
