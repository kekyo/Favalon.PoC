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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    public interface IAlgebraicCalculator
    {
        WidenedResult Widen(
            IExpression to, IExpression from);
        WidenedResult Widen(
            IExpression to, IExpression from,
            Func<IEnumerable<IExpression>, IExpression?> createOr,
            Func<IExpression, IExpression, WidenedResult> widen);
    }

    public class AlgebraicCalculator : IAlgebraicCalculator
    {
        protected AlgebraicCalculator()
        { }

        public virtual WidenedResult Widen(IExpression to, IExpression from) =>
            this.Widen(to, from, AndExpression.From, this.Widen);

        public virtual WidenedResult Widen(
            IExpression to, IExpression from,
            Func<IEnumerable<IExpression>, IExpression?> createOr,
            Func<IExpression, IExpression, WidenedResult> widen)
        {
            switch (to, from)
            {
                // int: int <-- int
                // IComparable: IComparable <-- IComparable
                // _[1]: _[1] <-- _[1]
                case (IExpression toExpression, IExpression fromExpression) when toExpression.ShallowEquals(fromExpression):
                    return WidenedResult.Success(toExpression);

                // (int & double): (int & double) <-- (int & double)
                // (int & double & string): (int & double & string) <-- (int & double)
                // (int & IComparable): (int & IComparable) <-- (int & string)
                // null: (int & double) <-- (int & double & string)
                // null: (int & IServiceProvider) <-- (int & double)
                // (int & _): (int & _) <-- (int & string)
                // (_[1] & _[2]): (_[1] & _[2]) <-- (_[2] & _[1])
                case (IAndExpression(IExpression[] toExpressions), IAndExpression(IExpression[] fromExpressions)):
                    Debug.Assert(toExpressions.Length >= 2);
                    Debug.Assert(fromExpressions.Length >= 2);
                    var widened1 = fromExpressions.
                        Select(rhsExpression =>
                            WidenedResult.Combine(toExpressions.Select(lhsExpression => widen(lhsExpression, rhsExpression)))).
                        Memoize();
                    if (widened1.Any(r => r.IsUnexpected))
                    {
                        return WidenedResult.Unexpected;
                    }
                    else
                    {
                        return widened1.All(r => r.Expressions != null) ?
                            WidenedResult.Success(to) :
                            WidenedResult.Empty;
                    }

                // (int & double): (int & double) <-- int
                // (int & IServiceProvider): (int & IServiceProvider) <-- int
                // (int & IComparable): (int & IComparable) <-- string
                // (int & _): (int & _) <-- string
                // (int & _[1]): (int & _[1]) <-- _[2]
                case (IAndExpression(IExpression[] toExpressions), IExpression _):
                    Debug.Assert(toExpressions.Length >= 2);
                    var widened3 = toExpressions.
                        Select(lhsExpression => widen(lhsExpression, from)).
                        Memoize();
                    if (widened3.Any(widened => widened.IsUnexpected))
                    {
                        return WidenedResult.Unexpected;
                    }
                    // Requirements: 1 or any terms widened.
                    else if (widened3.Any(widened => widened.Expression != null))
                    {
                        //return SumExpression.From(
                        //    terms3.Zip(toTerms, (term, lhsTerm) => term ?? lhsTerm).Distinct().Memoize(),
                        //    true);
                        return createOr(
                            widened3.Collect(widened => widened.Expression!)) is IExpression ex1 ?
                                WidenedResult.Success(ex1) :
                                WidenedResult.Empty;
                    }
                    // Couldn't widen: (int & double) <-- string
                    else
                    {
                        return WidenedResult.Empty;
                    }

                // null: int <-- (int & double)
                case (IExpression _, IAndExpression(IExpression[] fromExpressions)):
                    Debug.Assert(fromExpressions.Length >= 2);
                    var widened2 = fromExpressions.
                        Select(rhsExpression => widen(to, rhsExpression)).
                        Memoize();
                    if (widened2.Any(widened => widened.IsUnexpected))
                    {
                        return WidenedResult.Unexpected;
                    }
                    return widened2.All(widened => widened.Expression != null) ?
                        (createOr(widened2.Select(widened => widened.Expression!)) is IExpression ex2 ?
                            WidenedResult.Success(ex2) :
                            WidenedResult.Empty) :
                        WidenedResult.Empty;

                default:
                    return WidenedResult.Empty;
            }
        }

        public static readonly AlgebraicCalculator Instance =
            new AlgebraicCalculator();
    }
}
