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

namespace Favalet.Expressions
{
    public sealed class LambdaExpression : ValueExpression
    {
        private LambdaExpression(Expression parameter, Expression expression, Expression higherOrder) :
            base(higherOrder)
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

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint)
        {
            var higherOrder = environment.Unify(higherOrderHint, this.HigherOrder);
            var lambdaHigherOrder = higherOrder as LambdaExpression;

            var parameter = environment.Visit(this.Parameter, lambdaHigherOrder?.Parameter ?? UnspecifiedExpression.Instance);
            var expression = environment.Visit(this.Expression, lambdaHigherOrder?.Expression ?? UnspecifiedExpression.Instance);

            var newLambdaHigherOrder = environment.Unify(higherOrder,
                CreateRecursive(parameter.HigherOrder, expression.HigherOrder));

            return new LambdaExpression(parameter, expression, newLambdaHigherOrder);
        }

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var parameter = environment.Visit(this.Parameter);
            var expression = environment.Visit(this.Expression);
            var higherOrder = (this.HigherOrder != null) ? environment.Visit(this.HigherOrder) : null!;

            return new LambdaExpression(parameter, expression, higherOrder);
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

        internal static LambdaExpression Create(Expression parameter, Expression expression) =>
            new LambdaExpression(parameter, expression, UnspecifiedExpression.Instance);

        internal static LambdaExpression CreateRecursive(Expression parameter, Expression expression) =>
            (parameter.HigherOrder, expression.HigherOrder) switch
            {
                (Rank3Expression _, Expression _) => new LambdaExpression(parameter, expression, Rank3Expression.Instance),
                (Expression _, Rank3Expression _) => new LambdaExpression(parameter, expression, Rank3Expression.Instance),
                (KindExpression _, Expression _) => new LambdaExpression(parameter, expression,
                    CreateRecursive(parameter.HigherOrder, expression.HigherOrder)),
                (Expression _, KindExpression _) => new LambdaExpression(parameter, expression,
                    CreateRecursive(parameter.HigherOrder, expression.HigherOrder)),
                (TypeExpression _, Expression _) => new LambdaExpression(parameter, expression,
                    CreateRecursive(parameter.HigherOrder, expression.HigherOrder)),
                (Expression _, TypeExpression _) => new LambdaExpression(parameter, expression,
                    CreateRecursive(parameter.HigherOrder, expression.HigherOrder)),
                (UnspecifiedExpression _, UnspecifiedExpression _) => new LambdaExpression(parameter, expression,
                    unspecified),
                (UnspecifiedExpression _, Expression _) => new LambdaExpression(parameter, expression,
                    CreateRecursive(UnspecifiedExpression.Instance, expression.HigherOrder)),
                (Expression _, UnspecifiedExpression _) => new LambdaExpression(parameter, expression,
                    CreateRecursive(parameter.HigherOrder, UnspecifiedExpression.Instance)),
                (Expression _, null) => new LambdaExpression(parameter, expression,
                    CreateRecursive(parameter.HigherOrder, UnspecifiedExpression.Instance)),
                (null, Expression _) => new LambdaExpression(parameter, expression,
                    CreateRecursive(UnspecifiedExpression.Instance, expression.HigherOrder)),
                _ => new LambdaExpression(parameter, expression,
                    CreateRecursive(parameter.HigherOrder, expression.HigherOrder))
            };

        private static readonly LambdaExpression unspecified =
            new LambdaExpression(UnspecifiedExpression.Instance, UnspecifiedExpression.Instance, null!);
        private static readonly LambdaExpression rank3 =
            new LambdaExpression(Rank3Expression.Instance, Rank3Expression.Instance, null!);
        private static readonly LambdaExpression kind =
            new LambdaExpression(KindExpression.Instance, KindExpression.Instance, rank3);
        private static readonly LambdaExpression type =
            new LambdaExpression(TypeExpression.Instance, TypeExpression.Instance, kind);
    }
}
