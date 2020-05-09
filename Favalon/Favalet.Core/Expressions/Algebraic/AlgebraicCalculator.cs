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

using Favalet.Expressions.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    public interface IAlgebraicCalculator<TContext>
    {
        IExpression? Widen(
            IExpression? to,
            IExpression? from,
            TContext context);
    }

    public class AlgebraicCalculator<TContext> : IAlgebraicCalculator<TContext>
    {
        protected AlgebraicCalculator()
        { }

        public IExpression? Widen(
            IExpression? to,
            IExpression? from,
            TContext context)
        {
            if (to == null || from == null)
            {
                return null;
            }

            if (to is TerminationTerm || from is TerminationTerm)
            {
                return null;
            }
            if (to is UnspecifiedTerm || from is UnspecifiedTerm)
            {
                return null;
            }

            return this.WidenCore(to, from, context);
        }

        protected virtual IExpression? OrFrom(IEnumerable<IExpression> operands) =>
            OrExpression.From(operands);

        protected virtual IExpression? WidenCore(
            IExpression to,
            IExpression from,
            TContext context)
        {
            // int: int <-- int
            // IComparable: IComparable <-- IComparable
            // _[1]: _[1] <-- _[1]
            if (to.ShallowEquals(from))
            {
                return to;
            }

            // (int | double): (int | double) <-- (int | double)
            // (int | double | string): (int | double | string) <-- (int | double)
            // (int | IComparable): (int | IComparable) <-- (int | string)
            // null: (int | double) <-- (int | double | string)
            // null: (int | IServiceProvider) <-- (int | double)
            // (int | _): (int | _) <-- (int | string)
            // (_[1] | _[2]): (_[1] | _[2]) <-- (_[2] | _[1])
            if (to is IOrExpression(IExpression[] tss1) &&
                from is IOrExpression(IExpression[] fss1))
            {
                var results = fss1.
                    Select(fs => tss1.Any(ts => this.Widen(ts, fs, context) != null)).
                    Memoize();
                return results.All(r => r) ?
                    to : null;
            }

            // (int | double): (int | double) <-- int
            // (int | IServiceProvider): (int | IServiceProvider) <-- int
            // (int | IComparable): (int | IComparable) <-- string
            // (int | _): (int | _) <-- string
            // (int | _[1]): (int | _[1]) <-- _[2]
            if (to is IOrExpression(IExpression[] tss2))
            {
                var results = tss2.
                    Select(ts => this.Widen(ts, from, context)).
                    Memoize();
                return results.Any(r => r != null) ?
                    OrFrom(results.Where(r => r != null)!) : null;
            }

            // null: int <-- (int | double)
            if (from is IOrExpression(IExpression[] fss2))
            {
                var results = fss2.
                    Select(fs => this.Widen(to, fs, context)).
                    Memoize();
                return results.All(r => r != null) ?
                    OrFrom(results!) : null;
            }

            return null;
        }

        public static readonly AlgebraicCalculator<TContext> Instance =
            new AlgebraicCalculator<TContext>();
    }

    public static class AlgebraicCalculatorExtension
    {
        public static IExpression? Widen(
            this IAlgebraicCalculator<bool> calculator,
            IExpression? to,
            IExpression? from) =>
            calculator.Widen(to, from, false);
    }
}
