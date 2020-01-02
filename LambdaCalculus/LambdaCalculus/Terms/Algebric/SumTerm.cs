using LambdaCalculus.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms.Algebric
{
    public sealed class SumTerm : MultipleTerm<SumTerm>
    {
        internal SumTerm(Term[] terms) :
            base(terms)
        { }

        protected override string OnPrettyPrint(PrettyPrintContext context)
        {
            var terms = Utilities.Join(
                " + ",
                this.Terms.Select(term => $"({term.PrettyPrint(context)})"));
            return $"({terms})";
        }

        protected override Term Create(Term[] terms) =>
            Composed(terms)!;

        public static Term? Composed(IEnumerable<Term> terms)
        {
            var ts = terms.ToArray();
            return ts.Length switch
            {
                0 => null,
                1 => ts[0],
                _ => new SumTerm(ts)
            };
        }
    }
}
