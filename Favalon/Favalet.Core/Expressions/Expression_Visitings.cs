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

namespace Favalet.Expressions
{
    partial class Expression
    {
        protected internal interface IInferringEnvironment
        {
            Expression Unify(Expression expression1, Expression expression2);
            Expression Unify(Expression expression1, Expression expression2, Expression expression3);

            void Memoize(VariableExpression symbol, Expression expression);

            Expression? Lookup(VariableExpression symbol);

            TExpression Visit<TExpression>(TExpression expression, Expression higherOrderHint)
                where TExpression : Expression;
        }

        protected internal interface IResolvingEnvironment
        {
            Expression? Lookup(VariableExpression symbol);

            TExpression Visit<TExpression>(TExpression expression)
                where TExpression : Expression;
        }

        internal Expression InternalVisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            this.VisitInferring(environment, higherOrderHint);
        internal Expression InternalVisitResolving(IResolvingEnvironment environment) =>
            this.VisitResolving(environment);
    }
}
