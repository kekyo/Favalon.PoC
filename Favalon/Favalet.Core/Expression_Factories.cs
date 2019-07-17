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
using Favalet.Expressions.Specialized;

namespace Favalet
{
    partial class Expression
    {
        public static UnspecifiedExpression Unspecified =>
            UnspecifiedExpression.Instance;

        public static KindExpression Kind =>
            KindExpression.Instance;
        public static TypeExpression Type =>
            TypeExpression.Instance;

        public static LiteralExpression Literal(object value) =>
            new LiteralExpression(value, TypeExpression.Instance);

        public static SymbolicVariableExpression Free(string name, Expression higherOrder) =>
            new FreeVariableExpression(name, higherOrder);
        public static SymbolicVariableExpression Free(string name) =>
            new FreeVariableExpression(name, UnspecifiedExpression.Instance);

        public static BoundVariableExpression Bound(string name, Expression higherOrder) =>
            new BoundVariableExpression(name, higherOrder);
        public static BoundVariableExpression Bound(string name) =>
            new BoundVariableExpression(name, UnspecifiedExpression.Instance);

        public static ApplyExpression Apply(Expression function, Expression argument, Expression higherOrder) =>
            new ApplyExpression(function, argument, higherOrder);
        public static ApplyExpression Apply(Expression function, Expression argument) =>
            new ApplyExpression(function, argument, UnspecifiedExpression.Instance);

        public static LambdaExpression Lambda(BoundVariableExpression parameter, Expression expression) =>
            LambdaExpression.Create(parameter, expression, false);
        public static LambdaExpression Lambda(LambdaExpression parameter, Expression expression) =>
            LambdaExpression.Create(parameter, expression, false);

        public static BindExpression Bind(BoundVariableExpression bound, Expression expression, Expression higherOrder) =>
            new BindExpression(bound, expression, false, higherOrder);
        public static BindExpression Bind(BoundVariableExpression bound, Expression expression) =>
            new BindExpression(bound, expression, false, UnspecifiedExpression.Instance);

        public static BindExpression RecursiveBind(BoundVariableExpression bound, Expression expression, Expression higherOrder) =>
            new BindExpression(bound, expression, true, higherOrder);
        public static BindExpression RecursiveBind(BoundVariableExpression bound, Expression expression) =>
            new BindExpression(bound, expression, true, UnspecifiedExpression.Instance);
    }
}
