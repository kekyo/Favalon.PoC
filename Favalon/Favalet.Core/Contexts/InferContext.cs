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
using Favalet.Expressions.Specialized;
using System.Collections.Generic;

namespace Favalet.Contexts
{
    public interface IInferContext : IScopedTypeContext<IInferContext>
    {
        void Unify(IExpression to, IExpression from);
    }

    public interface IFixupContext
    {
        IExpression? Widen(IExpression to, IExpression from);

        IExpression? LookupPlaceholder(PlaceholderTerm placeholder);
    }

    internal interface IInternalInferContext : IInferContext
    {
        PlaceholderTerm CreatePlaceholder();

        int DrawNextPlaceholderIndex();

        IExpression? Substitute(
            PlaceholderTerm placeholder,
            IExpression from,
            bool isForward);
    }

    internal sealed class InferContext :
        TypeContext, IInternalInferContext, IFixupContext
    {
        private readonly IRootTypeContext rootContext;
        private readonly Dictionary<int, IExpression> descriptions;

        private InferContext(IRootTypeContext rootContext) :
            base(rootContext)
        {
            this.rootContext = rootContext;
            this.descriptions = new Dictionary<int, IExpression>();
        }

        private InferContext(InferContext parent) :
            base(parent)
        {
            this.rootContext = parent.rootContext;
            this.descriptions = parent.descriptions;
        }

        public IInferContext CreateDerivedScope() =>
            new InferContext(this);

        PlaceholderTerm IInternalInferContext.CreatePlaceholder() =>
            PlaceholderTerm.Create(this, 1);

        int IInternalInferContext.DrawNextPlaceholderIndex() =>
            this.rootContext.DrawNextPlaceholderIndex();

        IExpression? IInternalInferContext.Substitute(
            PlaceholderTerm placeholder,
            IExpression from,
            bool isForward)
        {
            if (this.descriptions.TryGetValue(placeholder.Index, out var lastCombined))
            {
                var result = isForward ?
                    this.rootContext.Features.Widen(lastCombined, from, this) :  // forward
                    this.rootContext.Features.Widen(from, lastCombined, this);   // backward

                if (result is IExpression)
                {
                    this.descriptions[placeholder.Index] = result;
                    return result;
                }
                else
                {
                    var combinedExpression = OverloadTerm.From(new[] { lastCombined, from });
                    this.descriptions[placeholder.Index] = combinedExpression!;
                    return null;
                }
            }
            else
            {
                this.descriptions.Add(placeholder.Index, from);
                return from;
            }
        }

        public void Unify(IExpression to, IExpression from) =>
            this.rootContext.Features.Widen(to, from, this);

        public IExpression? Widen(IExpression to, IExpression from) =>
            this.rootContext.Features.Widen(to, from, null);

        public IExpression? LookupPlaceholder(PlaceholderTerm placeholder)
        {
            var current = placeholder;
            while (true)
            {
                if (this.descriptions.TryGetValue(current.Index, out var combined))
                {
                    if (combined is PlaceholderTerm p)
                    {
                        current = p;
                    }
                    else
                    {
                        return combined;
                    }
                }
                else
                {
                    return null;
                }
            }

        }

        public static InferContext Create(IRootTypeContext rootContext) =>
            new InferContext(rootContext);
    }
}
