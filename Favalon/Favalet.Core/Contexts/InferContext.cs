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
using System.Linq;

namespace Favalet.Contexts
{
    public interface IInferContext : IScopedTypeContext<IInferContext>
    {
        IPlaceholderTerm CreatePlaceholder(IExpression higherOrder);

        void Unify(IExpression to, IExpression from);
    }

    public interface IFixupContext
    {
        IExpression? Widen(IExpression to, IExpression from);

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
            IPlaceholderTerm placeholder, IExpression from, Directions direction, bool isWiden)
        {
            if (this.descriptions.TryGetValue(placeholder.Index, out var description))
            {
                var combinedDirection = CorrectVariance(description.Direction, direction);

                var result = combinedDirection == Directions.Forward ?
                    this.InternalUnify(description.Expression, from, isWiden) :  // forward
                    this.InternalUnify(from, description.Expression, isWiden);   // backward

                if (result is IExpression)
                {
                    this.descriptions[placeholder.Index] =
                        new PlaceholderDescription(result, combinedDirection);
                    return result;
                }
                else if (!isWiden)
                {
                    var combinedExpression = OverloadTerm.From(new[] { description.Expression, from });
                    this.descriptions[placeholder.Index] =
                        new PlaceholderDescription(combinedExpression!, combinedDirection);
                    return combinedExpression;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                this.descriptions.Add(placeholder.Index, new PlaceholderDescription(from, direction));
                return from;
            }
        }

        private IExpression? InternalUnify(IExpression to, IExpression from, bool isWiden)
        {
            // int: int <-- int
            // IComparable: IComparable <-- IComparable
            // _[1]: _[1] <-- _[1]
            if (to.ExactEquals(from))
            {
                return to;
            }

            // int->object: int->object <-- object->int
            if (to is IFunctionDeclaredExpression(IExpression tp, IExpression tr) &&
                from is IFunctionDeclaredExpression(IExpression fp, IExpression fr))
            {
                var pr = this.InternalUnify(fp, tp, isWiden);
                var rr = this.InternalUnify(tr, fr, isWiden);

                if (pr != null && rr != null)
                {
                    return FunctionDeclaredExpression.From(pr, rr);
                }
                else
                {
                    return null;
                }
            }

            // (int + double): (int + double) <-- (int + double)
            // (int + double + string): (int + double + string) <-- (int + double)
            // (int + IComparable): (int + IComparable) <-- (int + string)
            // null: (int + double) <-- (int + double + string)
            // null: (int + IServiceProvider) <-- (int + double)
            // (int + _): (int + _) <-- (int + string)
            // (_[1] + _[2]): (_[1] + _[2]) <-- (_[2] + _[1])
            if (to is ISumExpression(IExpression[] tss1) &&
                from is ISumExpression(IExpression[] fss1))
            {
                var unified = fss1.
                    Select(fs => tss1.Any(ts => this.InternalUnify(ts, fs, isWiden) != null)).
                    Memoize();
                return unified.All(w => w) ?
                    to :
                    null;
            }

            // (int + double): (int + double) <-- int
            // (int + IServiceProvider): (int + IServiceProvider) <-- int
            // (int + IComparable): (int + IComparable) <-- string
            // (int + _): (int + _) <-- string
            // (int + _[1]): (int + _[1]) <-- _[2]
            if (to is ISumExpression(IExpression[] tss2))
            {
                var results = tss2.
                    Select(ts => this.InternalUnify(ts, from, isWiden)).
                    Where(result => result != null).
                    Memoize();
                return OverloadTerm.From(results!);
            }

            // null: int <-- (int + double)
            if (from is ISumExpression(IExpression[] fss2))
            {
                var results = fss2.
                    Select(fs => this.InternalUnify(to, fs, isWiden)).
                    Where(result => result != null).
                    Memoize();
                return OverloadTerm.From(results!);
            }

            // object: object <-- int
            // double: double <-- int
            // IComparable: IComparable <-- string
            if (to is ITypeTerm toType &&
                from is ITypeTerm fromType)
            {
                return toType.IsConvertibleFrom(fromType) ?
                    to :
                    null;
            }

            if (to is IPlaceholderTerm tph)
            {
                return this.InternalUnifyPlaceholder(tph, from, Directions.Forward, isWiden);
            }
            if (from is IPlaceholderTerm fph)
            {
                return this.InternalUnifyPlaceholder(fph, to, Directions.Reverse, isWiden);
            }

            return isWiden ?
                null :
                OverloadTerm.From(new[] { to, from });
        }

        public void Unify(IExpression to, IExpression from) =>
            this.InternalUnify(to, from, false);
        public IExpression? Widen(IExpression to, IExpression from) =>
            this.InternalUnify(to, from, true);

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
