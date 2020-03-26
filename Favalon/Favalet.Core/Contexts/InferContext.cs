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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalet.Contexts
{
    public interface IInferContext : IScopedTypeContext<IInferContext>
    {
        IPlaceholderTerm CreatePlaceholder(IExpression higherOrder);

        IExpression CalculateSum(IEnumerable<IExpression> expressions);

        bool Unify(IExpression to, IExpression from);
    }

    public interface IFixupContext
    {
        IExpression? Resolve(IPlaceholderTerm placeholder);
    }

    internal sealed class InferContext : TypeContext, IInferContext, IFixupContext
    {
        private struct PlaceholderDescription
        {
            public readonly IExpression Expression;
            public readonly bool IsForward;

            public PlaceholderDescription(IExpression expression, bool isForward)
            {
                this.Expression = expression;
                this.IsForward = isForward;
            }

            public override string ToString() =>
                this.IsForward ? $"{this.Expression} <== rhs" : $"lhs <== {this.Expression}";
        }

        private readonly IRootTypeContext rootContext;
        private readonly Dictionary<int, PlaceholderDescription> descriptions;

        private InferContext(IRootTypeContext rootContext) :
            base(rootContext)
        {
            this.rootContext = rootContext;
            this.descriptions = new Dictionary<int, PlaceholderDescription>();
        }

        private InferContext(InferContext parent) :
            base(parent)
        {
            this.rootContext = parent.rootContext;
            this.descriptions = parent.descriptions;
        }

        public IInferContext CreateDerivedScope() =>
            new InferContext(this);

        public IPlaceholderTerm CreatePlaceholder(IExpression higherOrder) =>
            PlaceholderTerm.Create(this.rootContext.DrawNextPlaceholderIndex(), higherOrder);

        public IExpression CalculateSum(IEnumerable<IExpression> expressions) =>
            SumExpression.From(
                expressions.Where(expression => !(expression is TerminationTerm)),
                false)!;

        private bool UnifyPlaceholder(IPlaceholderTerm placeholder, IExpression from, bool isForward)
        {
            if (this.descriptions.TryGetValue(placeholder.Index, out var description))
            {
                // Covariance correction
                //   true, true : forward
                //   true, false : backward
                //   false, true : backward
                //   false, false : forward

                var combinedForward = !(description.IsForward ^ isForward);

                var result = combinedForward ?
                    this.Unify(description.Expression, from) :   // forward
                    this.Unify(from, description.Expression);    // backward

                if (result)
                {
                    this.descriptions[placeholder.Index] =
                        combinedForward ?
                            new PlaceholderDescription(description.Expression, combinedForward) :
                            new PlaceholderDescription(from, combinedForward);
                }
                else
                {
                    var combinedExpression = this.CalculateSum(new[] { description.Expression, from });
                    this.descriptions[placeholder.Index] =
                        new PlaceholderDescription(combinedExpression, combinedForward);
                }

                return true;
            }
            else
            {
                this.descriptions.Add(placeholder.Index, new PlaceholderDescription(from, isForward));
                return true;
            }
        }

        public bool Unify(IExpression to, IExpression from)
        {
            if (to.ExactEquals(from))
            {
                return true;
            }

            if (to is IFunctionDeclaredExpression(IExpression tp, IExpression tr) &&
                from is IFunctionDeclaredExpression(IExpression fp, IExpression fr))
            {
                var pr = this.Unify(tp, fp);
                var rr = this.Unify(tr, fr);
                return pr && rr;
            }

            if (to is ISumExpression(IExpression[] tss))
            {
                var results = tss.Select(ts => this.Unify(ts, from)).Memoize();
                return results.Any();
            }
            if (from is ISumExpression(IExpression[] fss))
            {
                var results = fss.Select(fs => this.Unify(to, fs)).Memoize();
                return results.Any();
            }

            if (to is IPlaceholderTerm tph)
            {
                return this.UnifyPlaceholder(tph, from, true);
            }
            if (from is IPlaceholderTerm fph)
            {
                return this.UnifyPlaceholder(fph, to, false);
            }

            //if (this.rootContext.Features.Widen(to, from) is IExpression widen1)
            //{
            //    return this.Unify(widen1, from);
            //}
            //if (this.rootContext.Features.Widen(from, to) is IExpression widen2)
            //{
            //    return this.Unify(widen2, to);
            //}

            return false;
        }

        public IExpression? Resolve(IPlaceholderTerm placeholder)
        {
            var current = placeholder;
            while (true)
            {
                if (this.descriptions.TryGetValue(current.Index, out var description))
                {
                    if (description.Expression is IPlaceholderTerm p)
                    {
                        current = p;
                    }
                    else
                    {
                        return description.Expression;
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
