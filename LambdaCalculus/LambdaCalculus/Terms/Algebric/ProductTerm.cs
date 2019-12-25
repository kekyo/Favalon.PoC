﻿using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms.Algebric
{
    public sealed class ProductTerm : MultipleTerm<ProductTerm>
    {
        internal ProductTerm(Term[] terms) :
            base(terms)
        { }

        protected override Term Create(Term[] terms) =>
            Composed(terms)!;

        public static Term? Composed(IEnumerable<Term> terms)
        {
            var ts = terms.ToArray();
            return ts.Length switch
            {
                0 => null,
                1 => ts[0],
                _ => new ProductTerm(ts)
            };
        }
    }
}
