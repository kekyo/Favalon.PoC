using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LambdaCalculus
{
    public sealed class CompositionTerm : Term
    {
        public readonly Term[] Terms;

        private CompositionTerm(Term[] terms)
        {
            Debug.Assert(terms.Length >= 2);
            Debug.Assert(terms.Distinct().Count() == terms.Length);
            this.Terms = terms;
        }

        public override Term HigherOrder =>
            LambdaCalculus.UnspecifiedTerm.Instance;

        public override Term Reduce(ReduceContext context) =>
            Distinct(this.Terms.Select(term => term.Reduce(context)));

        public override Term Infer(InferContext context) =>
            Distinct(this.Terms.Select(term => term.Infer(context)));

        public override Term Fixup(InferContext context) =>
            Distinct(this.Terms.Select(term => term.Fixup(context)));

        public override bool Equals(Term? other) =>
            other is CompositionTerm rhs ?
                !this.Terms.Except(rhs.Terms).Any() :
                false;

        private static Term Distinct(IEnumerable<Term> terms)
        {
            var distincted = terms.Distinct().ToArray();
            return (distincted.Length == 1) ? distincted[0] : new CompositionTerm(distincted);
        }

        public static Term Compose(Term term1, Term term2)
        {
            var composition1 = term1 as CompositionTerm;
            var composition2 = term2 as CompositionTerm;

            if (composition1 is CompositionTerm)
            {
                if (composition2 is CompositionTerm)
                {
                    return Distinct(composition1.Terms.Concat(composition2.Terms));
                }
                else
                {
                    return Distinct(composition1.Terms.Concat(new[] { term2 }));
                }
            }
            else if (composition2 is CompositionTerm)
            {
                return Distinct(new[] { term1 }.Concat(composition2.Terms));
            }
            else
            {
                return Distinct(new[] { term1, term2 });
            }
        }
    }
}
