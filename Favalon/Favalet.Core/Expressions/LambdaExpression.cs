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
using System.ComponentModel;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public sealed class LambdaExpression : ValueExpression
    {
        private LambdaExpression(Expression parameter, Expression expression, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        public readonly Expression Parameter;
        public readonly Expression Expression;

        protected override FormattedString FormatReadableString(FormatContext context)
        {
            var arrow = (context.FormatOperator == FormatOperators.Fancy) ? "→" : "->";
            return FormattedString.RequiredEnclosing(
                $"{FormatReadableString(context, this.Parameter, true)} {arrow} {FormatReadableString(context, this.Expression, context.FormatNaming != FormatNamings.Friendly)}");
        }

        protected override Expression VisitInferring(IInferringContext context, Expression higherOrderHint)
        {
            var higherOrder = context.Unify(higherOrderHint, this.HigherOrder);

            var visitedParameter = higherOrder switch
            {
                LambdaExpression(Expression parameter, Expression _) => context.Visit(this.Parameter, parameter),
                _ => context.Visit(this.Parameter, UnspecifiedExpression.Instance),
            };

            var visitedExpression = higherOrder switch
            {
                LambdaExpression(Expression _, Expression expression) => context.Visit(this.Expression, expression),
                _ => context.Visit(this.Expression, UnspecifiedExpression.Instance),
            };

            var visitedHigherOrder = new LambdaExpression(
                visitedParameter.HigherOrder, visitedExpression.HigherOrder, UnspecifiedExpression.Instance, this.TextRange);
            if (!(higherOrder is LambdaExpression))
            {
                context.Unify(higherOrder, visitedHigherOrder);
            }

            return new LambdaExpression(visitedParameter, visitedExpression, higherOrder, this.TextRange);
        }

        protected override Expression VisitResolving(IResolvingContext context)
        {
            var parameter = context.Visit(this.Parameter);
            var expression = context.Visit(this.Expression);
            var higherOrder = context.Visit(this.HigherOrder);

            return new LambdaExpression(parameter, expression, higherOrder, this.TextRange);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out Expression parameter, out Expression expression)
        {
            parameter = this.Parameter;
            expression = this.Expression;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out Expression parameter, out Expression expression, out Expression higherOrder)
        {
            parameter = this.Parameter;
            expression = this.Expression;
            higherOrder = this.HigherOrder;
        }

        internal static LambdaExpression Create(
            Expression parameter, Expression expression, bool isRecursive, TextRange textRange)
        {
            Debug.Assert((parameter is Expression) && (expression is Expression));

            return (parameter, expression) switch
            {
                (UnspecifiedExpression _, UnspecifiedExpression _) =>
                    new LambdaExpression(parameter, expression, UnspecifiedExpression.Instance, textRange),
                (Expression _, UnspecifiedExpression _) =>
                    new LambdaExpression(parameter, UnspecifiedExpression.Instance, UnspecifiedExpression.Instance, textRange),
                (UnspecifiedExpression _, Expression _) =>
                    new LambdaExpression(UnspecifiedExpression.Instance, expression, UnspecifiedExpression.Instance, textRange),
                _ => new LambdaExpression(parameter, expression, isRecursive ?
                    (Expression)Create(parameter.HigherOrder, expression.HigherOrder, true, textRange) :
                    UnspecifiedExpression.Instance, textRange),
            };
        }

        internal static LambdaExpression CreateWithPlaceholder(
            IInferringContext context, Expression parameter, Expression expression, bool isRecursive, TextRange textRange)
        {
            Debug.Assert((parameter is Expression) && (expression is Expression));

            return (parameter, expression) switch
            {
                (UnspecifiedExpression _, UnspecifiedExpression _) =>
                    new LambdaExpression(parameter, expression, UnspecifiedExpression.Instance, textRange),
                (Expression _, UnspecifiedExpression _) =>
                    new LambdaExpression(
                        parameter,
                        context.CreatePlaceholder(UnspecifiedExpression.Instance, textRange),
                        isRecursive ?
                            (Expression)CreateWithPlaceholder(context, parameter.HigherOrder, UnspecifiedExpression.Instance, true, textRange) :
                            UnspecifiedExpression.Instance, textRange),
                (UnspecifiedExpression _, Expression _) =>
                    new LambdaExpression(
                        context.CreatePlaceholder(UnspecifiedExpression.Instance, textRange),
                        expression,
                        isRecursive ?
                            (Expression)CreateWithPlaceholder(context, UnspecifiedExpression.Instance, expression.HigherOrder, true, textRange) :
                            UnspecifiedExpression.Instance, textRange),
                _ => new LambdaExpression(
                    parameter,
                    expression,
                    isRecursive ?
                        (Expression)CreateWithPlaceholder(context, parameter.HigherOrder, expression.HigherOrder, true, textRange) :
                        UnspecifiedExpression.Instance, textRange),
            };
        }
    }
}
