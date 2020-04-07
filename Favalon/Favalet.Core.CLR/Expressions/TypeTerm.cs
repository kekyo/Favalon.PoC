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

using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public abstract class TypeTerm :
        Expression, ITerm
    {
        private static readonly Dictionary<Type, ITerm> types =
            new Dictionary<Type, ITerm>();

        public readonly Type Type;

        private protected TypeTerm(Type type) =>
            this.Type = type;

        public override bool Equals(IExpression? rhs) =>
            rhs is TypeTerm type &&
                this.Type.Equals(type.Type);

        public override int GetHashCode() =>
            this.Type.GetHashCode();

        public override string FormatString(IFormatStringContext context) =>
            context.UseSimpleLabel ?
                this.Type.GetFullName() :
                context.Format(this, this.Type.GetFullName());

        public static ITerm From(Type type)
        {
            // TODO: detect delegates
            // TODO: detect generics

            lock (types)
            {
                if (!types.TryGetValue(type, out var term))
                {
                    term = new ConcreteTypeTerm(type);
                    types.Add(type, term);
                }
                return term;
            }
        }
    }

    public sealed class ConcreteTypeTerm :
        TypeTerm
    {
        internal ConcreteTypeTerm(Type type) :
            base(type)
        { }

        public override IExpression HigherOrder =>
            ExpressionFactory.kindType;
    }
}
