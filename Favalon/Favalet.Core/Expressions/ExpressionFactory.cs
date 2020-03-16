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

using System;

namespace Favalet.Expressions
{
    public class ExpressionFactory : IExpressionFactory
    {
        protected ExpressionFactory()
        { }

        private static readonly IdentityTerm unspecified =
            IdentityTerm.Create("_", null!);

        public static IdentityTerm Unspecified() =>
            unspecified;

        public static IdentityTerm Identity(string identity) =>
            IdentityTerm.Create(identity, unspecified);

        public static ApplyExpression Apply(Expression function, Expression argument) =>
            ApplyExpression.Create(function, argument, unspecified);

        public static readonly IExpressionFactory Instance =
            new ExpressionFactory();

        Expression IExpressionFactory.Numeric(string value) =>
            throw new NotImplementedException();

        Expression IExpressionFactory.String(string value) =>
            throw new NotImplementedException();

        Expression IExpressionFactory.Identity(string identity) =>
            IdentityTerm.Create(identity, unspecified);

        Expression IExpressionFactory.Apply(Expression function, Expression argument) =>
            ApplyExpression.Create(function, argument, unspecified);
    }
}
