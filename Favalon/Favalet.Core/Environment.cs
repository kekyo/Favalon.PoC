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

using Favalet.Internals;
using Favalet.Expressions;
using Favalet.Expressions.Internals;
using Favalet.Expressions.Specialized;

namespace Favalet
{
    public sealed partial class Environment :
        Expression.IInferringEnvironment, Expression.IResolvingEnvironment
    {
        private readonly PlaceholderController placeholderController = new PlaceholderController();

        private Environment()
        { }

#line hidden
        public PlaceholderExpression CreatePlaceholder(Expression higherOrder, TextRange textRange) =>
            placeholderController.Create(higherOrder, textRange);

        void Expression.IInferringEnvironment.Memoize(VariableExpression symbol, Expression expression) =>
            placeholderController.Memoize(symbol, expression);

        Expression? Expression.IInferringEnvironment.Lookup(VariableExpression symbol) =>
            placeholderController.Lookup(symbol);
        Expression? Expression.IResolvingEnvironment.Lookup(VariableExpression symbol) =>
            placeholderController.Lookup(symbol);

        private TExpression Visit<TExpression>(TExpression expression, Expression higherOrderHint)
            where TExpression : Expression =>
            (TExpression)expression.InternalVisitInferring(this, higherOrderHint);
        TExpression Expression.IInferringEnvironment.Visit<TExpression>(TExpression expression, Expression higherOrderHint) =>
            (TExpression)expression.InternalVisitInferring(this, higherOrderHint);
        TExpression Expression.IResolvingEnvironment.Visit<TExpression>(TExpression expression) =>
            (TExpression)expression.InternalVisitResolving(this);

        public Expression Infer(Expression expression) =>
            this.Infer(expression, UnspecifiedExpression.Instance);

        public TExpression Infer<TExpression>(TExpression expression, Expression higherOrderHint)
            where TExpression : Expression =>
            (TExpression)this.Infer((Expression)expression, higherOrderHint);
        public TExpression Infer<TExpression>(TExpression expression)
            where TExpression : Expression =>
            (TExpression)this.Infer((Expression)expression, UnspecifiedExpression.Instance);
#line default

        public Expression Infer(Expression expression, Expression higherOrderHint)
        {
            var partial = expression.InternalVisitInferring(this, higherOrderHint);
            return partial.InternalVisitResolving(this);
        }

        public static Environment Create() =>
            new Environment();
    }
}
