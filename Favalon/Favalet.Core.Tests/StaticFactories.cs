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
using Favalet.Expressions.Additionals;
using Favalet.Expressions.Basis;
using Favalet.Expressions.Internals;
using Favalet.Expressions.Specialized;

namespace Favalet
{
    public static class StaticFactories
    {
        public static UnspecifiedExpression Unspecified =>
            Expression.Unspecified;

        public static KindExpression Kind =>
            Expression.Kind;
        public static TypeExpression Type =>
            Expression.Type;

        public static LiteralExpression Literal(object value) =>
            Expression.Literal(value, TextRange.Unknown);

        public static SymbolicVariableExpression Free(string name, Expression higherOrder) =>
            Expression.Free(name, higherOrder, TextRange.Unknown);
        public static SymbolicVariableExpression Free(string name) =>
            Expression.Free(name, Unspecified, TextRange.Unknown);

        public static SymbolicVariableExpression Implicit(string name, Expression higherOrder) =>
            ImplicitVariableExpression.Create(name, higherOrder, TextRange.Unknown);
        public static SymbolicVariableExpression Implicit(string name) =>
            ImplicitVariableExpression.Create(name, Unspecified, TextRange.Unknown);

        public static BoundVariableExpression Bound(string name, Expression higherOrder) =>
            Expression.Bound(name, higherOrder, TextRange.Unknown);
        public static BoundVariableExpression Bound(string name) =>
            Expression.Bound(name, Unspecified, TextRange.Unknown);

        public static ApplyExpression Apply(Expression function, Expression argument, Expression higherOrder) =>
            Expression.Apply(function, argument, higherOrder, TextRange.Unknown);
        public static ApplyExpression Apply(Expression function, Expression argument) =>
            Expression.Apply(function, argument, Unspecified, TextRange.Unknown);

        public static LambdaExpression Lambda(BoundVariableExpression parameter, Expression expression) =>
            Expression.Lambda(parameter, expression, TextRange.Unknown);
        public static LambdaExpression Lambda(LambdaExpression parameter, Expression expression) =>
            Expression.Lambda(parameter, expression, TextRange.Unknown);

        public static BindExpression Bind(BoundVariableExpression bound, Expression expression, Expression higherOrder) =>
            Expression.Bind(bound, expression, higherOrder, TextRange.Unknown);
        public static BindExpression Bind(BoundVariableExpression bound, Expression expression) =>
            Expression.Bind(bound, expression, Unspecified, TextRange.Unknown);

        public static BindExpression RecursiveBind(BoundVariableExpression bound, Expression expression, Expression higherOrder) =>
            Expression.RecursiveBind(bound, expression, higherOrder, TextRange.Unknown);
        public static BindExpression RecursiveBind(BoundVariableExpression bound, Expression expression) =>
            Expression.RecursiveBind(bound, expression, Unspecified, TextRange.Unknown);
    }
}
