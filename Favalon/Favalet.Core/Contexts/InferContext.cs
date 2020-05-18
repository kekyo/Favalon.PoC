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
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Collections.Generic;

namespace Favalet.Contexts
{
    public interface IInferContext : IScopedTypeContext<IInferContext>
    {
        IPlaceholderTerm CreatePlaceholder();
        void Unify(IExpression to, IExpression from);
    }

    public interface IFixupContext
    {
        WidenedResult Widen(IExpression to, IExpression from);
        IExpression? LookupPlaceholder(IPlaceholderTerm placeholder);
    }

    internal sealed class InferContext :
        TypeContext, IInferContext, IFixupContext
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

        internal int DrawNextPlaceholderIndex() =>
            this.rootContext.DrawNextPlaceholderIndex();

        public IPlaceholderTerm CreatePlaceholder() =>
            PlaceholderTerm.Create(this, 1);

        private WidenedResult Substitute(
            IPlaceholderTerm placeholder,
            IExpression from,
            bool isForward)
        {
            if (this.descriptions.TryGetValue(placeholder.Index, out var lastCombined))
            {
                var result = isForward ?
                    this.Solve(lastCombined, from) :  // forward
                    this.Solve(from, lastCombined);   // backward

                if (result.Expression is IExpression)
                {
                    this.descriptions[placeholder.Index] = result.Expression;
                    return result;
                }
                else
                {
                    var combinedExpression = OverloadTerm.From(new[] { lastCombined, from });
                    this.descriptions[placeholder.Index] = combinedExpression!;
                    return WidenedResult.Nothing();
                }
            }
            else
            {
                this.descriptions.Add(placeholder.Index, from);
                return WidenedResult.Success(from);
            }
        }

        private WidenedResult Solve(IExpression to, IExpression from)
        {
            if (this.rootContext.Features.Widen(
                to,
                from,
                exs => OverloadTerm.From(exs)?.InferIfRequired(this),
                this.Solve) is WidenedResult widened &&
                widened.Expression != null)
            {
                return widened;
            }

            if (to is IPlaceholderTerm tph)
            {
                return this.Substitute(tph, from, true);
            }
            if (from is IPlaceholderTerm fph)
            {
                return this.Substitute(fph, to, false);
            }

            return WidenedResult.Nothing();
        }

        public void Unify(IExpression to, IExpression from) =>
            this.Solve(to, from);

        public WidenedResult Widen(IExpression to, IExpression from) =>
            this.rootContext.Features.Widen(to, from);

        public IExpression? LookupPlaceholder(IPlaceholderTerm placeholder)
        {
            var current = placeholder;
            while (true)
            {
                if (this.descriptions.TryGetValue(current.Index, out var combined))
                {
                    if (combined is IPlaceholderTerm p)
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
