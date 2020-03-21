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
using System.Diagnostics;

namespace Favalet.Contexts
{
    public enum ExpressionVariances
    {
        Invariance,
        Covariance,
        Contravariance
    }

    public interface IInferContext : IScopedTypeContext<IInferContext>
    {
        IIdentityTerm CreatePlaceholder(IExpression higherOrder);

        bool Unify(IExpression to, IExpression from, ExpressionVariances variant);
    }

    internal sealed class InferContext : TypeContext, IInferContext
    {
        private struct Description
        {
            public readonly IExpression Expression;
            public readonly ExpressionVariances Variance;

            public Description(IExpression expression, ExpressionVariances variance)
            {
                this.Expression = expression;
                this.Variance = variance;
            }
        }

        private readonly IRootTypeContext rootContext;
        private readonly Dictionary<int, Description> descriptions;

        private InferContext(IRootTypeContext rootContext) :
            base(rootContext)
        {
            this.rootContext = rootContext;
            this.descriptions = new Dictionary<int, Description>();
        }

        private InferContext(InferContext parent) :
            base(parent)
        {
            this.rootContext = parent.rootContext;
            this.descriptions = parent.descriptions;
        }

        public IInferContext CreateDerivedScope() =>
            new InferContext(this);

        public IIdentityTerm CreatePlaceholder(IExpression higherOrder) =>
            PlaceholderTerm.Create(this.rootContext.DrawNextPlaceholderIndex(), higherOrder);

        private bool UnifyPlaceholder(PlaceholderTerm placeholder, IExpression from, ExpressionVariances variance)
        {
            if (this.descriptions.TryGetValue(placeholder.Index, out var description))
            {
                switch ((description.Variance, variance))
                {
                    case (ExpressionVariances.Contravariance, _):
                    case (ExpressionVariances.Covariance, _):
                    case (_, ExpressionVariances.Contravariance):
                    case (_, ExpressionVariances.Covariance):
                        throw new NotImplementedException();
                    default:
                        return this.Unify(description.Expression, from, variance);
                };
            }
            else
            {
                this.descriptions.Add(
                    placeholder.Index, new Description(from, variance));
                return true;
            }
        }

        public bool Unify(IExpression to, IExpression from, ExpressionVariances variance)
        {
            switch (variance)
            {
                case ExpressionVariances.Covariance:
                    if (this.rootContext.Features.Widen(to, from) is IExpression widen1)
                    {
                        return true;
                    }
                    break;
                case ExpressionVariances.Contravariance:
                    if (this.rootContext.Features.Widen(from, to) is IExpression widen2)
                    {
                        return true;
                    }
                    break;
                default:
                    if (to.ExactEquals(from))
                    {
                        return true;
                    }
                    break;
            }

            if (to is IFunctionDeclaredExpression(IExpression tp, IExpression tr) &&
                from is IFunctionDeclaredExpression(IExpression fp, IExpression fr))
            {
                var pr = this.Unify(tp, fp, variance);
                var rr = this.Unify(tr, fr, variance);
                return pr && rr;
            }

            if (to is PlaceholderTerm tph)
            {
                return this.UnifyPlaceholder(tph, from, variance);
            }
            if (from is PlaceholderTerm fph)
            {
                var inverted = variance switch
                {
                    ExpressionVariances.Covariance => ExpressionVariances.Contravariance,
                    ExpressionVariances.Contravariance => ExpressionVariances.Covariance,
                    _ => variance
                };
                return this.UnifyPlaceholder(fph, to, inverted);
            }

            return false;
        }

        public static InferContext Create(IRootTypeContext rootContext) =>
            new InferContext(rootContext);
    }
}
