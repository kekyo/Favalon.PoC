﻿////////////////////////////////////////////////////////////////////////////
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
using System.Collections.Generic;
using System.Linq;

namespace Favalet.Contexts
{
    public interface ITypeContext
    {
        void MutableBind(
            string identity,
            IExpression expression,
            BoundTermAttributes attributes = BoundTermAttributes.Prefix | BoundTermAttributes.LeftToRight,
            BoundTermPrecedences precedence = BoundTermPrecedences.Function);

        BoundInformation[] LookupBoundInformations(IIdentityTerm identity);
    }

    public interface IScopedTypeContext<TContext> : ITypeContext
        where TContext : ITypeContext
    {
        TContext CreateDerivedScope();
    }

    internal interface IInternalTypeContext
    {
        IEnumerable<(int, BoundInformation)> RecursiveLookupBoundInformations(IIdentityTerm identity, int distance);
    }

    public abstract class TypeContext : ITypeContext, IInternalTypeContext
    {
        private readonly ITypeContext? parent;
        private Dictionary<string, Dictionary<IExpression, BoundInformation>>? bounds;

        private protected TypeContext(ITypeContext? parent) =>
            this.parent = parent;

        public void MutableBind(
            string identity,
            IExpression expression,
            BoundTermAttributes attributes,
            BoundTermPrecedences precedence)
        {
            if (this.bounds == null)
            {
                this.bounds = new Dictionary<string, Dictionary<IExpression, BoundInformation>>();
            }

            if (!this.bounds.TryGetValue(identity, out var overloads))
            {
                overloads = new Dictionary<IExpression, BoundInformation>();
                this.bounds.Add(identity, overloads);
            }

            overloads[expression.HigherOrder] =
                new BoundInformation(expression, attributes, precedence);
        }

        public BoundInformation[] LookupBoundInformations(IIdentityTerm identity) =>
            ((IInternalTypeContext)this).RecursiveLookupBoundInformations(identity, 0).
            GroupBy(result => result.Item2.Expression.HigherOrder).
            Select(g => g.OrderBy(result => result.Item1).First()).
            Select(result => result.Item2).
            Memoize();

        IEnumerable<(int, BoundInformation)> IInternalTypeContext.RecursiveLookupBoundInformations(IIdentityTerm identity, int distance)
        {
            var collected = Enumerable.Empty<(int, BoundInformation)>();

            if (this.bounds is Dictionary<string, Dictionary<IExpression, BoundInformation>> bounds)
            {
                if (bounds.TryGetValue(identity.Identity, out var overloads))
                {
                    collected = overloads.Select(entry => (distance, entry.Value));
                }
            }

            if (this.parent is IInternalTypeContext parent)
            {
                collected = collected.Concat(parent.RecursiveLookupBoundInformations(identity, distance + 1));
            }

            return collected;
        }
    }
}
