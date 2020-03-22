﻿////////////////////////////////////////////////////////////////////////////
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

using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    public interface IAlgebraicCalculator
    {
        IExpression? Widen(IExpression to, IExpression from);
    }

    public class AlgebraicCalculator : IAlgebraicCalculator
    {
        protected AlgebraicCalculator()
        { }

        public virtual IExpression? Widen(IExpression? to, IExpression? from)
        {
            switch (to, from)
            {
                // int: int <-- int
                // IComparable: IComparable <-- IComparable
                // _[1]: _[1] <-- _[1]
                case (IExpression toExpression, IExpression fromExpression) when toExpression.Equals(fromExpression):
                    return toExpression;

                // (int + double): (int + double) <-- (int + double)
                // (int + double + string): (int + double + string) <-- (int + double)
                // (int + IComparable): (int + IComparable) <-- (int + string)
                // null: (int + double) <-- (int + double + string)
                // null: (int + IServiceProvider) <-- (int + double)
                // (int + _): (int + _) <-- (int + string)
                // (_[1] + _[2]): (_[1] + _[2]) <-- (_[2] + _[1])
                case (ISumExpression(IExpression[] toExpressions), ISumExpression(IExpression[] fromExpressions)):
                    var widened1 = fromExpressions.
                        Select(rhsExpression => toExpressions.Any(lhsExpression => this.Widen(lhsExpression, rhsExpression) != null)).
                        Memoize();
                    return widened1.All(w => w) ?
                        to :
                        null;

                // null: int <-- (int + double)
                case (IExpression _, ISumExpression(IExpression[] fromExpressions)):
                    Debug.Assert(fromExpressions.Length >= 2);
                    var expressions2 = fromExpressions.
                        Select(rhsExpression => this.Widen(to, rhsExpression)).
                        Memoize();
                    return expressions2.All(expression => expression != null) ?
                        SumExpression.From(expressions2.Distinct().Memoize()!, true) :
                        null;

                // (int + double): (int + double) <-- int
                // (int + IServiceProvider): (int + IServiceProvider) <-- int
                // (int + IComparable): (int + IComparable) <-- string
                // (int + _): (int + _) <-- string
                // (int + _[1]): (int + _[1]) <-- _[2]
                case (ISumExpression(IExpression[] toExpressions), IExpression _):
                    Debug.Assert(toExpressions.Length >= 2);
                    var expressions3 = toExpressions.
                        Select(lhsExpression => this.Widen(lhsExpression, from)).
                        Memoize();
                    // Requirements: 1 or any terms widened.
                    if (expressions3.Any(expression => expression != null))
                    {
                        //return SumExpression.From(
                        //    terms3.Zip(toTerms, (term, lhsTerm) => term ?? lhsTerm).Distinct().Memoize(),
                        //    true);
                        return SumExpression.From(
                            expressions3.Where(expression => expression != null).Distinct().Memoize()!, true);
                    }
                    // Couldn't narrow: (int + double) <-- string
                    else
                    {
                        return null;
                    }

                default:
                    return null;
            }
        }

        public static readonly AlgebraicCalculator Instance =
            new AlgebraicCalculator();
    }
}
