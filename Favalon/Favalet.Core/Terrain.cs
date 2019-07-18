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
using Favalet.Expressions.Basis;
using Favalet.Expressions.Internals;
using Favalet.Expressions.Specialized;
using System.Collections.Generic;

namespace Favalet
{
    public sealed partial class Terrain :
        Expression.IInferringContext, Expression.IResolvingContext
    {
        private readonly PlaceholderController placeholderController = new PlaceholderController();
        private readonly List<InferErrorInformation> errorInformations = new List<InferErrorInformation>();

        private Terrain()
        { }

#line hidden
        private PlaceholderExpression CreatePlaceholder(Expression higherOrder, TextRange textRange) =>
            placeholderController.Create(higherOrder, textRange);
        PlaceholderExpression Expression.IInferringContext.CreatePlaceholder(Expression higherOrder, TextRange textRange) =>
            placeholderController.Create(higherOrder, textRange);

        public void Bind(BoundVariableExpression bound, Expression expression) =>
            placeholderController.Memoize(bound, expression);
        void Expression.IInferringContext.Memoize(SymbolicVariableExpression symbol, Expression expression) =>
            placeholderController.Memoize(symbol, expression);

        Expression? Expression.IInferringContext.Lookup(VariableExpression symbol) =>
            placeholderController.Lookup(this, symbol);
        Expression? Expression.IResolvingContext.Lookup(VariableExpression symbol) =>
            placeholderController.Lookup(this, symbol);

        private TExpression Visit<TExpression>(TExpression expression, Expression higherOrderHint)
            where TExpression : Expression =>
            (TExpression)expression.InternalVisitInferring(this, higherOrderHint);
        TExpression Expression.IInferringContext.Visit<TExpression>(TExpression expression, Expression higherOrderHint) =>
            (TExpression)expression.InternalVisitInferring(this, higherOrderHint);
        TExpression Expression.IResolvingContext.Visit<TExpression>(TExpression expression) =>
            (TExpression)expression.InternalVisitResolving(this);

        internal Expression RecordError(string details, Expression primaryExpression, params Expression[] expressions)
        {
            errorInformations.Add(InferErrorInformation.Create(details, primaryExpression, expressions));
            return primaryExpression;
        }

        Expression Expression.IInferringContext.RecordError(string details, Expression primaryExpression, Expression[] expressions) =>
            this.RecordError(details, primaryExpression, expressions);

        public InferResult<Expression> Infer(Expression expression, Expression higherOrderHint) =>
            this.Infer<Expression>(expression, higherOrderHint);
#line default

        public InferResult<TExpression> Infer<TExpression>(TExpression expression, Expression higherOrderHint)
            where TExpression : Expression
        {
            var partial = expression.InternalVisitInferring(this, higherOrderHint);
            var inferred = partial.InternalVisitResolving(this);
            var result = InferResult<TExpression>.Create((TExpression)inferred, errorInformations.ToArray());
            errorInformations.Clear();
            return result;
        }

        public static Terrain Create() =>
            new Terrain();
    }
}
