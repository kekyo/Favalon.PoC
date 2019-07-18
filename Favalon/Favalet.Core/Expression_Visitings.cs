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

namespace Favalet
{
    partial class Expression
    {
        protected internal interface IInferringContext
        {
            PlaceholderExpression CreatePlaceholder(Expression higherOrder, TextRange textRange);

            Expression Unify(Expression expression1, Expression expression2);
            void Memoize(VariableExpression symbol, Expression expression);

            Expression? Lookup(VariableExpression symbol);

            Expression RecordError(string details, Expression primaryExpression, params Expression[] expressions);

            TExpression Visit<TExpression>(TExpression expression, Expression higherOrderHint)
                where TExpression : Expression;
        }

        protected internal interface IResolvingContext
        {
            Expression? Lookup(VariableExpression symbol);

            TExpression Visit<TExpression>(TExpression expression)
                where TExpression : Expression;
        }

#line hidden
        internal Expression InternalVisitInferring(IInferringContext context, Expression higherOrderHint) =>
            this.VisitInferring(context, higherOrderHint);
        internal Expression InternalVisitResolving(IResolvingContext context) =>
            this.VisitResolving(context);
#line default
    }
}
