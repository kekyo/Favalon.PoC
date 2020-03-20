////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using Favalet.Expressions.Specialized;
using System;

namespace Favalet.Expressions
{
    public class ExpressionFactory : IExpressionFactory
    {
        protected ExpressionFactory()
        { }

        internal static readonly IdentityTerm fourthType =
            IdentityTerm.Create("#", TerminationTerm.Instance);

        internal static readonly IdentityTerm kindType =
            IdentityTerm.Create("*", fourthType);

        public static UnspecifiedTerm Unspecified() =>
            UnspecifiedTerm.Instance;

        public static IdentityTerm FourthType() =>
            fourthType;

        public static IdentityTerm KindType() =>
            kindType;

        public static IdentityTerm Identity(string identity) =>
            IdentityTerm.Create(identity, UnspecifiedTerm.Instance);

        public static ApplyExpression Apply(IExpression function, IExpression argument) =>
            ApplyExpression.Create(function, argument, UnspecifiedTerm.Instance);

        public static IExpression FunctionDeclaration(IExpression parameter, IExpression result) =>
            FunctionDeclaredExpression.From(parameter, result);

        public static readonly IExpressionFactory Instance =
            new ExpressionFactory();

        IExpression IExpressionFactory.Numeric(string value) =>
            throw new NotImplementedException();

        IExpression IExpressionFactory.String(string value) =>
            throw new NotImplementedException();

        IExpression IExpressionFactory.Identity(string identity) =>
            IdentityTerm.Create(identity, UnspecifiedTerm.Instance);

        IExpression IExpressionFactory.Apply(IExpression function, IExpression argument) =>
            ApplyExpression.Create(function, argument, UnspecifiedTerm.Instance);
    }
}
