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

using Favalet.Expressions;

namespace Favalet.Contexts
{
    public interface IInferContext : ITypeContext
    {
        void Unify(IExpression to, IExpression from);

        IInferContext NewScope();
    }

    internal sealed class InferContext : IInferContext
    {
        private sealed class BoundPair
        {
            public readonly string Identity;
            public readonly IExpression Expression;

            public BoundPair(string identity, IExpression expression)
            {
                this.Identity = identity;
                this.Expression = expression;
            }
        }

        private readonly ITypeContext parent;
        private BoundPair? boundPair;

        public InferContext(ITypeContext parent) =>
            this.parent = parent;

        public IExpression? Lookup(IIdentityTerm identity) =>
            (boundPair?.Identity.Equals(identity) ?? false) ?
                boundPair!.Expression :
                parent.Lookup(identity);

        public IInferContext NewScope() =>
            new InferContext(this);

        public void Unify(IExpression to, IExpression from)
        {
        }
    }
}
