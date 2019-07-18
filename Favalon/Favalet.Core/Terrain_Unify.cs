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

using Favalet.Expressions;
using Favalet.Expressions.Specialized;
using System;
using System.Diagnostics;

namespace Favalet
{
    partial class Terrain
    {
        private Expression UnifyLambda(LambdaExpression lambda1, LambdaExpression lambda2)
        {
            Debug.Assert((lambda1 is Expression) && (lambda2 is Expression));

            this.Unify(lambda1.Parameter, lambda2.Parameter);
            this.Unify(lambda1.Expression, lambda2.Expression);

            return lambda2;
        }

        private Expression UnifyLambda(LambdaExpression lambda, Expression expression)
        {
            Debug.Assert((lambda is Expression) && (expression is Expression));

            var newLambda = LambdaExpression.CreateWithPlaceholder(
                this,
                this.Unify(lambda.Parameter, UnspecifiedExpression.Instance),
                this.Unify(lambda.Expression, UnspecifiedExpression.Instance),
                true,
                lambda.TextRange);

            if (expression is PlaceholderExpression placeholder)
            {
                placeholderController.Memoize(placeholder, newLambda);
            }

            return lambda;
        }

        private Expression UnifyPlaceholder(
            PlaceholderExpression placeholder, Expression expression)
        {
            Debug.Assert((placeholder is Expression) && (expression is Expression));

            if (placeholderController.Lookup(this, placeholder) is Expression lookup)
            {
                return this.Unify(lookup, expression);
            }

            if (!(expression is UnspecifiedExpression))
            {
                placeholderController.Memoize(placeholder, expression);
            }

            return placeholder;
        }

        private Expression Unify(Expression expression1, Expression expression2)
        {
            Debug.Assert((expression1 is Expression) && (expression2 is Expression));

            var result = (expression1, expression2) switch
            {
                (UnspecifiedExpression _, UnspecifiedExpression _) =>
                    this.CreatePlaceholder(UnspecifiedExpression.Instance, expression1.TextRange),

                (Expression _, Expression _) when expression1.Equals(expression2) =>
                    expression1,

                (UnspecifiedExpression _, PlaceholderExpression placeholder) =>
                    this.UnifyPlaceholder(placeholder, expression1),
                (PlaceholderExpression placeholder, UnspecifiedExpression _) =>
                    this.UnifyPlaceholder(placeholder, expression2),

                (Expression _, PlaceholderExpression placeholder) =>
                    this.UnifyPlaceholder(placeholder, expression1),
                (PlaceholderExpression placeholder, Expression _) =>
                    this.UnifyPlaceholder(placeholder, expression2),

                (Expression _, UnspecifiedExpression _) =>
                    expression1,
                (UnspecifiedExpression _, Expression _) =>
                    expression2,

                (LambdaExpression lambda1, LambdaExpression lambda2) =>
                    this.UnifyLambda(lambda1, lambda2),
                (Expression _, LambdaExpression lambda) =>
                    this.UnifyLambda(lambda, expression1),
                (LambdaExpression lambda, Expression _) =>
                    this.UnifyLambda(lambda, expression2),

                _ => this.RecordError(
                    $"Cannot unify: between \"{expression1.ReadableString}\" and \"{expression2.ReadableString}\"",
                    expression1,
                    expression2)
            };

            return result;
        }

#line hidden
        Expression Expression.IInferringContext.Unify(Expression expression1, Expression expression2) =>
            this.Unify(expression1, expression2);
#line default
    }
}
