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
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions
{
    public sealed class OverloadTerm :
        Expression, ITerm, ISumExpression, IInferrableExpression, IReducibleExpression
    {
        public readonly IExpression[] Overloads;

        private OverloadTerm(IExpression[] overloads, IExpression higherOrder)
        {
            this.Overloads = overloads;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        IExpression[] ISumExpression.Expressions =>
            this.Overloads;

        public IExpression Infer(IInferContext context)
        {
            var higherOrder = this.HigherOrder.InferIfRequired(context);
            var overloads = this.Overloads.
                Select(overload => overload.InferIfRequired(context, 2)).
                Distinct().
                Memoize();

            var overloadHigherOrders = From(
                overloads.Select(overload => overload.HigherOrder),
                () => context.CreatePlaceholder(UnspecifiedTerm.Instance))!;

            context.Unify(higherOrder, overloadHigherOrders);

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Overloads.ExactSequenceEqual(overloads))
            {
                return this;
            }
            else
            {
                Debug.Assert(overloads.Length >= 1);

                return From(overloads, () => higherOrder)!;
            }
        }

        public IExpression Fixup(IFixupContext context)
        {
            var higherOrder = this.HigherOrder.FixupIfRequired(context);
            var overloads = this.Overloads.
                Select(overload => overload.FixupIfRequired(context)).
                Distinct().
                Memoize();

            var valids = overloads.
                Select(overload => (overload, higherOrder: context.Widen(higherOrder, overload.HigherOrder)!)).
                Where(entry => entry.higherOrder != null).
                Distinct().
                Memoize();

            if (valids.Length == 0)
            {
                if (this.HigherOrder.ExactEquals(higherOrder))
                {
                    return this;
                }
                else
                {
                    return From(this.Overloads, () => higherOrder)!;
                }
            }

            var validOverloads = valids.
                Select(entry => entry.overload).
                OrderBy(overload => overload.GetHashCode()).
                Memoize();
            var validHigherOrders = valids.
                Select(entry => entry.higherOrder).
                OrderBy(overload => overload.GetHashCode()).
                Memoize();

            var validHigherOrder = From(validHigherOrders)!;

            if (this.HigherOrder.ExactEquals(validHigherOrder) &&
                this.Overloads.ExactSequenceEqual(validOverloads))
            {
                return this;
            }
            else
            {
                return From(validOverloads, () => validHigherOrder)!;
            }
        }

        public IExpression Reduce(IReduceContext context)
        {
            var higherOrder = this.HigherOrder.ReduceIfRequired(context);
            var overloads = this.Overloads.
                Select(overload => overload.ReduceIfRequired(context)).
                Distinct().
                Memoize();

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Overloads.ExactSequenceEqual(overloads))
            {
                return this;
            }
            else
            {
                Debug.Assert(overloads.Length >= 1);

                return From(overloads, () => higherOrder)!;
            }
        }

        public override bool Equals(IExpression? rhs) =>
            rhs is OverloadTerm overload &&
                this.Overloads.SequenceEqual(overload.Overloads);

        public override int GetHashCode() =>
            this.Overloads.Aggregate(0, (agg, e) => agg ^ e.GetHashCode());

        public override T Format<T>(IFormatContext<T> context) =>
            context.Format(this, FormatOptions.Standard, this.Overloads);

        private static IExpression? From(
            IEnumerable<IExpression> overloads, Func<IExpression> higherOrder)
        {
            // It digs only first depth.
            var oes = overloads.
                SelectMany(overload => (overload is OverloadTerm(IExpression[] oes)) ? oes : new[] { overload }).
                Where(overload => !(overload is TerminationTerm)).  // Suppress termination
                Distinct().
                OrderBy(overload => overload.GetHashCode()).   // Make stable
                Memoize();

            return oes.Length switch
            {
                0 => null,
                1 => oes[0],
                _ => new OverloadTerm(oes, higherOrder())
            };
        }

        public static IExpression? From(IEnumerable<IExpression> overloads) =>
            From(overloads, () => UnspecifiedTerm.Instance);
    }
}
