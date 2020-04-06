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

        bool Unify(IExpression to, IExpression from);
    }

    public interface IFixupContext
    {
        ITypeContextFeatures Features { get; }

        IExpression? Resolve(IPlaceholderTerm placeholder);
    }

    internal sealed class InferContext :
        TypeContext, IInferContext, IFixupContext
    {
        private enum Directions
        {
            Forward,
            Reverse
        }

        private struct PlaceholderDescription
        {
            public readonly IExpression Expression;
            public readonly Directions Direction;

            public PlaceholderDescription(IExpression expression, Directions direction)
            {
                this.Expression = expression;
                this.Direction = direction;
            }

            public override string ToString() =>
                $"{this.Expression},{this.Direction}";
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

        ITypeContextFeatures IFixupContext.Features =>
            this.rootContext.Features;

        public IInferContext CreateDerivedScope() =>
            new InferContext(this);

        public IPlaceholderTerm CreatePlaceholder(IExpression higherOrder) =>
            PlaceholderTerm.Create(this.rootContext.DrawNextPlaceholderIndex(), higherOrder);

        private static Directions CorrectVariance(Directions a, Directions b) =>
            (a, b) switch
            {
                (Directions.Forward, Directions.Forward) => Directions.Forward,
                (Directions.Forward, Directions.Reverse) => Directions.Reverse,
                (Directions.Reverse, Directions.Forward) => Directions.Reverse,
                _ => Directions.Forward
            };

        private IExpression? InternalUnifyPlaceholder(
            IPlaceholderTerm placeholder, IExpression from, Directions direction)
        {
            if (this.descriptions.TryGetValue(placeholder.Index, out var description))
            {
                var combinedDirection = CorrectVariance(description.Direction, direction);

                var result = combinedDirection == Directions.Forward ?
                    this.InternalUnify(description.Expression, from) :  // forward
                    this.InternalUnify(from, description.Expression);   // backward

                if (result is IExpression)
                {
                    this.descriptions[placeholder.Index] =
                        new PlaceholderDescription(result, combinedDirection);
                    return result;
                }
                else
                {
                    var combinedExpression = OverloadTerm.From(new[] { description.Expression, from });
                    this.descriptions[placeholder.Index] =
                        new PlaceholderDescription(combinedExpression!, combinedDirection);
                    return combinedExpression;
                }
            }
            else
            {
                this.descriptions.Add(placeholder.Index, new PlaceholderDescription(from, direction));
                return from;
            }
        }

        private IExpression? InternalUnify(IExpression to, IExpression from)
        {
            if (to.ExactEquals(from))
            {
                return to;
            }

            if (to is IFunctionDeclaredExpression(IExpression tp, IExpression tr) &&
                from is IFunctionDeclaredExpression(IExpression fp, IExpression fr))
            {
                var pr = this.InternalUnify(tp, fp);
                var rr = this.InternalUnify(tr, fr);

                if (pr != null && rr != null)
                {
                    return FunctionDeclaredExpression.From(pr, rr);
                }
                else
                {
                    return null;
                }
            }

            if (to is ISumExpression(IExpression[] tss))
            {
                var results = tss.
                    Select(ts => this.InternalUnify(ts, from)).
                    Where(result => result != null).
                    Memoize();
                return OverloadTerm.From(results!);
            }
            if (from is ISumExpression(IExpression[] fss))
            {
                var results = fss.
                    Select(fs => this.InternalUnify(to, fs)).
                    Where(result => result != null).
                    Memoize();
                return OverloadTerm.From(results!);
            }

            if (to is IPlaceholderTerm tph)
            {
                return this.InternalUnifyPlaceholder(tph, from, Directions.Forward);
            }
            if (from is IPlaceholderTerm fph)
            {
                return this.InternalUnifyPlaceholder(fph, to, Directions.Reverse);
            }

            return null;
        }

        public bool Unify(IExpression to, IExpression from) =>
            this.InternalUnify(to, from) is IExpression;

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
