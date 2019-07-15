// This is part of Favalon project - https://github.com/kekyo/Favalon
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
            Debug.Assert((lambda1 is Expression) && (expression2 is Expression));

            var (lambda2Parameter, lambda2Expression) = expression2 is LambdaExpression lambda2 ?
                (lambda2.Parameter, lambda2.Expression) :
                (UnspecifiedExpression.Instance, UnspecifiedExpression.Instance);

            var parameter = this.Unify(lambda1.Parameter, lambda2Parameter);
            var expression = this.Unify(lambda1.Expression, lambda2Expression);

            var visitedParameter = this.Visit(parameter, UnspecifiedExpression.Instance);
            var visitedExpression = this.Visit(expression, UnspecifiedExpression.Instance);

            var newLambda = LambdaExpression.Create(visitedParameter, visitedExpression);

            if (expression2 is PlaceholderExpression placeholder)
            {
                placehoderController.Memoize(placeholder, newLambda);
            }
            else if (!(expression2 is KindExpression || expression2 is TypeExpression))
            {
                Debug.Assert(!(expression2 is LambdaExpression));
                throw new ArgumentException($"Cannot unify: between \"{lambda1.StrictReadableString}\" and \"{expression2.StrictReadableString}\"");
            }

            return newLambda;
        }

        private Expression UnifyPlaceholder(PlaceholderExpression placeholder, Expression expression)
        {
            Debug.Assert((placeholder is Expression) && (expression is Expression));

            var fixedExpression = (placeholder.HigherOrder, expression) switch
            {
                (UnspecifiedExpression _, Rank3Expression _) => throw new InvalidOperationException(),
                (UnspecifiedExpression _, KindExpression _) => placeholder.InternalCloneWithHigherOrder(Rank3Expression.Instance),
                (UnspecifiedExpression _, TypeExpression _) => placeholder.InternalCloneWithHigherOrder(KindExpression.Instance),
                (Rank3Expression _, KindExpression _) => placeholder,
                (KindExpression _, TypeExpression _) => placeholder,
                _ => expression
            };

            placehoderController.Memoize(placeholder, fixedExpression);
            return fixedExpression;
        }

        private Expression Unify2(Expression expression1, Expression expression2)
        {
            Debug.Assert((expression1 is Expression) && (expression2 is Expression));
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

            if (expression1 is LambdaExpression lambda1)
            {
                return this.UnifyLambda(lambda1, expression2);
            }
            else if (expression2 is LambdaExpression lambda2)
            {
                return this.UnifyLambda(lambda2, expression1);
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
            Debug.Assert((expression1 is Expression) && (expression2 is Expression));

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
            Debug.Assert((expression1 is Expression) && (expression2 is Expression) && (expression3 is Expression));

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
