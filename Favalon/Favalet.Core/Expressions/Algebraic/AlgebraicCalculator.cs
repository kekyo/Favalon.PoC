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
                case (IExpression toTerm, IExpression fromTerm) when toTerm.Equals(fromTerm):
                    return toTerm;

                // (int + double): (int + double) <-- (int + double)
                // (int + double + string): (int + double + string) <-- (int + double)
                // (int + IComparable): (int + IComparable) <-- (int + string)
                // null: (int + double) <-- (int + double + string)
                // null: (int + IServiceProvider) <-- (int + double)
                // (int + _): (int + _) <-- (int + string)
                // (_[1] + _[2]): (_[1] + _[2]) <-- (_[2] + _[1])
                case (ISumExpression(IExpression[] toTerms), ISumExpression(IExpression[] fromTerms)):
                    var terms1 = fromTerms.
                        Select(rhsTerm => toTerms.Any(lhsTerm => this.Widen(lhsTerm, rhsTerm) != null)).
                        Memoize();
                    return terms1.All(term => term) ?
                        to :
                        null;

                // null: int <-- (int + double)
                case (IExpression _, ISumExpression(IExpression[] fromTerms)):
                    Debug.Assert(fromTerms.Length >= 2);
                    var terms2 = fromTerms.
                        Select(rhsTerm => this.Widen(to, rhsTerm)).
                        Memoize();
                    return terms2.All(term => term != null) ?
                        SumExpression.From(terms2.Distinct().Memoize()!, true) :
                        null;

                // (int + double): (int + double) <-- int
                // (int + IServiceProvider): (int + IServiceProvider) <-- int
                // (int + IComparable): (int + IComparable) <-- string
                // (int + _): (int + _) <-- string
                // (int + _[1]): (int + _[1]) <-- _[2]
                case (ISumExpression(IExpression[] toTerms), IExpression _):
                    Debug.Assert(toTerms.Length >= 2);
                    var terms3 = toTerms.
                        Select(lhsTerm => this.Widen(lhsTerm, from)).
                        Memoize();
                    // Requirements: 1 or any terms widened.
                    if (terms3.Any(term => term != null))
                    {
                        //return SumExpression.From(
                        //    terms3.Zip(toTerms, (term, lhsTerm) => term ?? lhsTerm).Distinct().Memoize(),
                        //    true);
                        return SumExpression.From(
                            terms3.Where(term => term != null).Distinct().Memoize()!, true);
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
