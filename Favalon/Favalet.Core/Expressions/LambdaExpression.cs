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
                Create(parameter.HigherOrder, expression.HigherOrder));

            return new LambdaExpression(parameter, expression, newLambdaHigherOrder);
        }

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var parameter = environment.Visit(this.Parameter);
            var expression = environment.Visit(this.Expression);
            var higherOrder = environment.Visit(this.HigherOrder);

            return new LambdaExpression(parameter, expression, higherOrder);
        }

        public void Deconstruct(out Expression parameter, out Expression expression)
        {
            parameter = this.Parameter;
            expression = this.Expression;
        }

        public void Deconstruct(out Expression parameter, out Expression expression, out Expression higherOrder)
        {
            parameter = this.Parameter;
            expression = this.Expression;
            higherOrder = this.HigherOrder;
        }

        internal static LambdaExpression Create(Expression parameter, Expression expression) =>
            (parameter.HigherOrder, expression.HigherOrder) switch
            {
                (Rank3Expression _, Expression _) => throw new InvalidOperationException(),
                (Expression _, Rank3Expression _) => throw new InvalidOperationException(),
                (KindExpression _, Expression _) => new LambdaExpression(parameter, expression, KindExpression.Instance),
                (Expression _, KindExpression _) => new LambdaExpression(parameter, expression, KindExpression.Instance),
                (TypeExpression _, Expression _) => new LambdaExpression(parameter, expression, TypeExpression.Instance),
                (Expression _, TypeExpression _) => new LambdaExpression(parameter, expression, TypeExpression.Instance),
                (UnspecifiedExpression _, UnspecifiedExpression _) => Unspecified,
                _ => new LambdaExpression(parameter, expression, Create(parameter.HigherOrder, expression.HigherOrder))
            };

        private static new readonly LambdaExpression Unspecified =
            new LambdaExpression(UnspecifiedExpression.Instance, UnspecifiedExpression.Instance, null!);

        static LambdaExpression() =>
            Unspecified.InternalSetHigherOrder(Unspecified);
    }
}
