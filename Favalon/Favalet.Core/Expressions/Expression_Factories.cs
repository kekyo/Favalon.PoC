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

using Favalet.Expressions.Additionals;
using Favalet.Expressions.Basis;
using Favalet.Expressions.Specialized;

namespace Favalet.Expressions
{
    partial class Expression
    {
        public static UnspecifiedExpression Unspecified =>
            UnspecifiedExpression.Instance;

        public static KindExpression Kind =>
            KindExpression.Instance;
        public static TypeExpression Type =>
            TypeExpression.Instance;

        public static LiteralExpression Literal(object value, TextRange textRange) =>
            new LiteralExpression(value, TypeExpression.Instance, textRange);

        public static SymbolicVariableExpression Free(string name, Expression higherOrder, TextRange textRange) =>
            new FreeVariableExpression(name, higherOrder, textRange);

        public static BoundVariableExpression Bound(string name, Expression higherOrder, TextRange textRange) =>
            new BoundVariableExpression(name, higherOrder, textRange);

        public static ApplyExpression Apply(Expression function, Expression argument, Expression higherOrder, TextRange textRange) =>
            new ApplyExpression(function, argument, higherOrder, textRange);

        public static LambdaExpression Lambda(BoundVariableExpression parameter, Expression expression, TextRange textRange) =>
            LambdaExpression.Create(parameter, expression, false, textRange);
        public static LambdaExpression Lambda(LambdaExpression parameter, Expression expression, TextRange textRange) =>
            LambdaExpression.Create(parameter, expression, false, textRange);

        public static BindExpression Bind(BoundVariableExpression bound, Expression expression, Expression higherOrder, TextRange textRange) =>
            new BindExpression(bound, expression, false, higherOrder, textRange);

        public static BindExpression RecursiveBind(BoundVariableExpression bound, Expression expression, Expression higherOrder, TextRange textRange) =>
            new BindExpression(bound, expression, true, higherOrder, textRange);

        public static Expression? operator +(Expression? lhs, Expression? rhs) =>
            lhs.Apply(rhs);
    }
}
