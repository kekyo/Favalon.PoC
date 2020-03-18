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
using System.Collections.Generic;

namespace Favalet.Expressions
{
    public abstract class TypeTerm :
        Term, IConstantTerm
    {
        private static readonly Dictionary<FunctionTypeSignatureKey, FunctionTypeTerm> functionTypes =
            new Dictionary<FunctionTypeSignatureKey, FunctionTypeTerm>();

        private static readonly Dictionary<Type, ConcreteTypeTerm> concreteTypes =
            new Dictionary<Type, ConcreteTypeTerm>();

        public readonly Type Type;

        object IConstantTerm.Value =>
            this.Type;

        private protected TypeTerm(Type type) =>
            this.Type = type;

        public override bool Equals(IExpression? rhs) =>
            rhs is TypeTerm type &&
                this.Type.Equals(type.Type);

        public static FunctionTypeTerm FromFunction(Type result, params Type[] parameters)
        {
            lock (functionTypes)
            {
                var key = new FunctionTypeSignatureKey(result, parameters);
                if (!functionTypes.TryGetValue(key, out var term))
                {
                    term = new FunctionTypeTerm(
                        typeof(Func<,>).MakeGenericType(parameters[0], result));
                    functionTypes.Add(key, term);
                }
                return term;
            }
        }

        public static TypeTerm From(Type type)
        {
            // TODO: detect delegates

            lock (concreteTypes)
            {
                if (!concreteTypes.TryGetValue(type, out var term))
                {
                    term = new ConcreteTypeTerm(type);
                    concreteTypes.Add(type, term);
                }
                return term;
            }
        }
    }

    public sealed class ConcreteTypeTerm : TypeTerm
    {
        internal ConcreteTypeTerm(Type type) :
            base(type)
        { }

        public override IExpression HigherOrder =>
            ExpressionFactory.KindType();
    }

    public sealed class FunctionTypeTerm : TypeTerm
    {
        internal FunctionTypeTerm(Type type) :
            base(type)
        { }

        public override IExpression HigherOrder =>
            ExpressionFactory.KindType();  // TODO:
    }
}
