﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms.Types
{
    public class TypeCalculator : IComparer<Term>
    {
        protected TypeCalculator()
        { }

        public virtual bool IsAssignable(TypeTerm toType, TypeTerm fromType) =>
            toType.Equals(fromType);

        public virtual int Compare(Term x, Term y) =>
            (this.Widening(x, y) != null) ? -1 : 0;

        public Term? Widening(Term lhs, Term rhs)
        {
            switch ((lhs, rhs))
            {
                // int: int <-- int
                // IComparable: IComparable <-- IComparable
                // _[1]: _[1] <-- _[1]
                case (_, _) when object.ReferenceEquals(lhs, rhs) || lhs.Equals(rhs):
                    return lhs;

                // _[1]: _[1] <-- _[2]
                case (PlaceholderTerm placeholder, PlaceholderTerm _):
                    return placeholder;

                // _: _ <-- int
                // _: _ <-- (int | double)
                case (PlaceholderTerm placeholder, _):
                    return placeholder;

                // (int | double): (int | double) <-- (int | double)
                // (int | double | string): (int | double | string) <-- (int | double)
                // (int | IComparable): (int | IComparable) <-- (int | string)
                // null: (int | double) <-- (int | double | string)
                // null: (int | IServiceProvider) <-- (int | double)
                // (int | _): (int | _) <-- (int | string)
                // (_[1] | _[2]): (_[1] | _[2]) <-- (_[2] | _[1])
                case (SumTypeTerm(Term[] lhsTerms), SumTypeTerm(Term[] rhsTerms)):
                    var terms1 = rhsTerms.
                        Select(rhsTerm => lhsTerms.Any(lhsTerm => Widening(lhsTerm, rhsTerm) != null)).
                        ToArray();
                    return terms1.All(term => term) ?
                        lhs :
                        null;

                // object: object <-- int
                // IComparable: IComparable <-- string
                case (TypeTerm lhsType, TypeTerm rhsTerm):
                    return this.IsAssignable(lhsType, rhsTerm) ?
                        lhs :
                        null;

                // null: int <-- (int | double)
                case (_, SumTypeTerm(Term[] rhsTerms)):
                    var terms2 = rhsTerms.
                        Select(rhsTerm => Widening(lhs, rhsTerm)).
                        ToArray();
                    return terms2.All(term => term != null) ?
                        this.Sum(terms2!) :
                        null;

                // (int | double): (int | double) <-- int
                // (int | IServiceProvider): (int | IServiceProvider) <-- int
                // (int | IComparable): (int | IComparable) <-- string
                // (int | _): (int | _) <-- string
                // (int | _[1]): (int | _[1]) <-- _[2]
                case (SumTypeTerm(Term[] lhsTerms), _):
                    var terms3 = lhsTerms.
                        Select(lhsTerm => Widening(lhsTerm, rhs)).
                        ToArray();
                    // Requirements: 1 or any terms narrowed.
                    if (terms3.Any(term => term != null))
                    {
                        terms3 = terms3.
                            Zip(lhsTerms, (term, lhsTerm) => term ?? lhsTerm).
                            ToArray();
                        return terms3.Length switch
                        {
                            0 => null,
                            1 => terms3[0],
                            _ => this.Sum(terms3!)
                        };
                    }
                    // Couldn't narrow: (int | double) <-- string
                    else
                    {
                        return null;
                    }

                // null: int <-- _   [TODO: maybe]
                case (_, PlaceholderTerm placeholder):
                    return null;

                default:
                    return null;
            }
        }

        public Term Sum(Term lhs, Term rhs) =>
            SumTypeTerm.Create(lhs, rhs, this);

        public Term Sum(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(this.Sum),
                _ => throw new InvalidOperationException()
            };

        public static readonly TypeCalculator Instance =
            new TypeCalculator();
    }
}
