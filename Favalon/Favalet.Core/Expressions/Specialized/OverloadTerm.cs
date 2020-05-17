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
using Favalet.Expressions.Comparer;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Specialized
{
    public sealed class OverloadTerm :
        OperatorExpression<IOrExpression>, IOrExpression
    {
        private OverloadTerm(IExpression[] operands, IExpression higherOrder) : 
            base(operands, higherOrder)
        {
#if DEBUG
            var r = operands.OrderBy(oper => oper, ExpressionComparer.Instance).Memoize();
            var r2 = r.OrderBy(oper => oper, ExpressionComparer.Instance).Memoize();
            Debug.Assert(operands.SequenceEqual(r, ExactEqualityComparer.Instance));
#endif
        }

        protected override IExpression? From(
            IEnumerable<IExpression> operands,
            IExpression higherOrder) =>
            InternalFrom(
                operands.OrderBy(oper => oper, ExpressionComparer.Instance).Memoize(),
                higherOrder);

        public override IExpression Fixup(IFixupContext context)
        {
            var higherOrder = this.HigherOrder.FixupIfRequired(context);
            var overloads = this.Operands.
                Select(operand => operand.FixupIfRequired(context)).
                Distinct(LogicalEqualityComparer.Instance).
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
                    return InternalFrom(this.Operands, higherOrder)!;
                }
            }

            var validOverloads = valids.
                Select(entry => entry.overload).
                OrderBy(overload => overload, ExpressionComparer.Instance).   // make stable
                Memoize();
            var validHigherOrders = valids.
                Select(entry => entry.higherOrder).
                OrderBy(overload => overload, ExpressionComparer.Instance).   // make stable
                Memoize();

            var validHigherOrder = InternalFrom(validHigherOrders, UnspecifiedTerm.Instance)!;

            if (this.HigherOrder.ExactEquals(validHigherOrder) &&
                this.Operands.ExactSequenceEqual(validOverloads))
            {
                return this;
            }
            else
            {
                return InternalFrom(validOverloads, validHigherOrder)!;
            }
        }

        private static IExpression? InternalFrom(IEnumerable<IExpression> operands, IExpression higherOrder) =>
            From(
                operands,
                ops => new OverloadTerm(ops.OrderBy(oper => oper, ExpressionComparer.Instance).Memoize(), higherOrder),
                false);

        internal static IExpression? From(IEnumerable<IExpression> operands) =>
            InternalFrom(operands, UnspecifiedTerm.Instance);
    }
}
