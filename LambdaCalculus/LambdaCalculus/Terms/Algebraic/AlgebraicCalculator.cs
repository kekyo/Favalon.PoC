using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalon.Terms.Algebraic
{
    public class AlgebraicCalculator : IComparer<Term>
    {
        protected AlgebraicCalculator()
        { }

        protected virtual Term Sum(IEnumerable<Term> terms) =>
            TermFactory.Sum(terms)!;

        public virtual Term? Widening(Term to, Term from)
        {
            switch (to, from)
            {
                // int: int <-- int
                // IComparable: IComparable <-- IComparable
                // _[1]: _[1] <-- _[1]
                case (_, _) when to.Equals(from):
                    return to;

                // (int + double): (int + double) <-- (int + double)
                // (int + double + string): (int + double + string) <-- (int + double)
                // (int + IComparable): (int + IComparable) <-- (int + string)
                // null: (int + double) <-- (int + double + string)
                // null: (int + IServiceProvider) <-- (int + double)
                // (int + _): (int + _) <-- (int + string)
                // (_[1] + _[2]): (_[1] + _[2]) <-- (_[2] + _[1])
                case (SumTerm(Term[] toTerms), SumTerm(Term[] fromTerms)):
                    var terms1 = fromTerms.
                        Select(rhsTerm => toTerms.Any(lhsTerm => Widening(lhsTerm, rhsTerm) != null)).
                        ToArray();
                    return terms1.All(term => term) ?
                        to :
                        null;

                // null: int <-- (int + double)
                case (_, SumTerm(Term[] fromTerms)):
                    Debug.Assert(fromTerms.Length >= 2);
                    var terms2 = fromTerms.
                        Select(rhsTerm => Widening(to, rhsTerm)).
                        ToArray();
                    return terms2.All(term => term != null) ?
                        this.Sum(terms2!) :
                        null;

                // (int + double): (int + double) <-- int
                // (int + IServiceProvider): (int + IServiceProvider) <-- int
                // (int + IComparable): (int + IComparable) <-- string
                // (int + _): (int + _) <-- string
                // (int + _[1]): (int + _[1]) <-- _[2]
                case (SumTerm(Term[] toTerms), _):
                    Debug.Assert(toTerms.Length >= 2);
                    var terms3 = toTerms.
                        Select(lhsTerm => Widening(lhsTerm, from)).
                        ToArray();
                    // Requirements: 1 or any terms widened.
                    if (terms3.Any(term => term != null))
                    {
                        return this.Sum(terms3.
                            Zip(toTerms, (term, lhsTerm) => term ?? lhsTerm).
                            ToArray());
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

        public virtual int Compare(Term x, Term y)
        {
            if (x.Equals(y))
            {
                return 0;
            }

            var widened = this.Widening(x, y);
            return (widened is Term) ? -1 : 1;
        }

        public static readonly AlgebraicCalculator Instance =
            new AlgebraicCalculator();
    }
}
