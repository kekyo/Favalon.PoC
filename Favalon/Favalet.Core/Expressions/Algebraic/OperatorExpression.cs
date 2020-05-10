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
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using Favalet.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Favalet.Expressions.Algebraic
{
    public interface IOperatorExpression :
        IExpression
    {
        IExpression[] Operands { get; }

        IExpression From(IEnumerable<IExpression> operands);
    }

    public abstract class OperatorExpression<TOperator> :
        Expression, IOperatorExpression, IInferrableExpression, IReducibleExpression, IExpressionComparable
        where TOperator : class, IOperatorExpression
    {
        public readonly IExpression[] Operands;

        protected OperatorExpression(IExpression[] expressions, IExpression higherOrder)
        {
            this.Operands = expressions;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression[] IOperatorExpression.Operands =>
            this.Operands;

        protected abstract TOperator Create(
            IExpression[] operands,
            IExpression higherOrder);

        protected static IExpression? From(
            IEnumerable<IExpression> operands,
            Func<IExpression[], TOperator> creator,
            bool isStrict = true)
        {
            // It digs only first depth.
            var ops = isStrict ?
                operands.
                    SelectMany(operand => operand is TOperator(IExpression[] operands) ?
                        operands : new[] { operand }).
                    Memoize() :
                operands.
                    SelectMany(operand => operand is TOperator(IExpression[] operands) ?
                        operands : new[] { operand }).
                    Distinct().     // Exact expression type equality
                    Memoize();

            return ops.Length switch
            {
                0 => null,
                1 when !isStrict => ops[0],
                _ => creator(ops)
            };
        }

        private IExpression From(IEnumerable<IExpression> operands) =>
            From(operands, ops => Create(ops, UnspecifiedTerm.Instance))!;

        IExpression IOperatorExpression.From(IEnumerable<IExpression> operands) =>
            From(operands, ops => Create(ops, UnspecifiedTerm.Instance))!;

        public IExpression Infer(IInferContext context)
        {
            var higherOrder = this.HigherOrder.InferIfRequired(context);
            var operands = this.Operands.
                Select(operand => operand.InferIfRequired(context)).
                Distinct(ExactEqualityComparer.Instance).
                Memoize();

            var operandHigherOrders = this.From(
                operands.Select(operand => operand.HigherOrder));

            context.Unify(higherOrder, operandHigherOrders);

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Operands.ExactSequenceEqual(operands))
            {
                return this;
            }
            else
            {
                return From(operands, ops => this.Create(ops, higherOrder))!;
            }
        }

        public virtual IExpression Fixup(IFixupContext context)
        {
            var higherOrder = this.HigherOrder.FixupIfRequired(context);
            var operands = this.Operands.
                Select(operand => operand.FixupIfRequired(context)).
                Distinct(ExactEqualityComparer.Instance).
                Memoize();

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Operands.ExactSequenceEqual(operands))
            {
                return this;
            }
            else
            {
                return From(operands, ops => this.Create(ops, higherOrder))!;
            }
        }

        public virtual IExpression Reduce(IReduceContext context)
        {
            var higherOrder = this.HigherOrder.ReduceIfRequired(context);
            var operands = this.Operands.
                Select(operand => operand.ReduceIfRequired(context)).
                Distinct(ExactEqualityComparer.Instance).
                Memoize();

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Operands.ExactSequenceEqual(operands))
            {
                return this;
            }
            else
            {
                return From(operands, ops => this.Create(ops, higherOrder))!;
            }
        }

        public override sealed int GetHashCode() =>
            this.Operands.Aggregate(0, (agg, e) => agg ^ e.GetHashCode());

        public override sealed bool Equals(IExpression? rhs) =>
            rhs is TOperator oper &&
                this.Operands.SequenceEqual(oper.Operands, ShallowEqualityComparer.Instance);

        public override sealed T Format<T>(IFormatContext<T> context) =>
            context.Format(this, FormatOptions.SuppressHigherOrder, this.Operands);

        public virtual int CompareTo(IExpression rhs, IComparer<IExpression> comparer) =>
            rhs is IOperatorExpression op ?
                Operands.Zip(op.Operands, (lhs, rhs) => comparer.Compare(lhs, rhs)).
                FirstOrDefault(r => r != 0) :
            -1;
    }

    public static class OperatorExpressionExtension
    {
#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Deconstruct(this IOperatorExpression oper, out IExpression[] operands) =>
            operands = oper.Operands;
    }
}
