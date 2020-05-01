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
using System.Reflection;

namespace Favalet.Expressions
{
    public abstract class TypeTerm :
        Expression, ITypeTerm
    {
        private static readonly Dictionary<Type, ITerm> types =
            new Dictionary<Type, ITerm>();

        public readonly Type Type;

        private protected TypeTerm(Type type) =>
            this.Type = type;

        public bool IsConvertibleFrom(TypeTerm? from) =>
            from is TypeTerm && this.Type.IsConvertibleFrom(from.Type);

        bool ITypeTerm.IsConvertibleFrom(ITypeTerm from) =>
            this.IsConvertibleFrom(from as TypeTerm);

        public override bool Equals(IExpression? rhs) =>
            rhs is TypeTerm type &&
                this.Type.Equals(type.Type);

        public override int GetHashCode() =>
            this.Type.GetHashCode();

        public static ITerm From(Type type)
        {
            // TODO: detect delegates
            // TODO: detect generics

            lock (types)
            {
                if (!types.TryGetValue(type, out var term))
                {
                    // Special case: Force replacing RuntimeType to Type
                    if (typeof(Type).IsAssignableFrom(type))
                    {
                        term = ExpressionFactory.kindType;
                    }
                    else
                    {
                        term = new ConcreteTypeTerm(type);
                    }

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

        public override T Format<T>(IFormatContext<T> context) =>
            context.Format(
                this,
                FormatOptions.SuppressHigherOrder,
                this.Type.GetFullName());
    }
}
