﻿namespace Favalon.Terms.AlgebricData
{
    public sealed class SumTerm : MultipleTerm<SumTerm>
    {
        internal SumTerm(Term[] terms) :
            base(terms)
        { }

        protected override Term Create(Term[] terms) =>
            new SumTerm(terms);
    }
}
