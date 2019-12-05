using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalon.Algebric
{
    public sealed class ProductTerm : Term
    {
        public readonly Term[] Terms;

        private ProductTerm(Term[] terms) =>
            this.Terms = terms;

        public override Term HigherOrder =>
            new ProductTerm(this.Terms.Select(term => term.HigherOrder).ToArray());

        public override Term Reduce(ReduceContext context)
        {
            var terms = this.Terms.Aggregate(
                Enumerable.Empty<Term>(),
                (agg, term) => term is ProductTerm product ?
                    agg.Concat(product.Terms) :
                    agg.Concat(new[] { term })).
                ToArray();

            Debug.Assert(terms.Length >= 2);
            return new ProductTerm(terms);
        }

        public override Term Infer(InferContext context) =>
            new ProductTerm(this.Terms.Select(term => term.Infer(context)).ToArray());

        public override Term Fixup(FixupContext context) =>
            new ProductTerm(this.Terms.Select(term => term.Fixup(context)).ToArray());

        public override bool Equals(Term? other) =>
            other is ProductTerm rhs ? rhs.Terms.SequenceEqual(this.Terms) : false;

        public static ProductTerm Create(Term term0, Term term1) =>
            new ProductTerm(new[] { term0, term1 });
        public static ProductTerm Create(Term term0, Term term1, IEnumerable<Term> terms) =>
            new ProductTerm(new[] { term0, term1 }.Concat(terms).ToArray());
    }
}
