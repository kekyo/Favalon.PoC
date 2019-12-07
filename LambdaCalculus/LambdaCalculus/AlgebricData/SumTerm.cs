using System.Diagnostics;
using System.Linq;

namespace Favalon.AlgebricData
{
    public sealed class SumTerm : Term
    {
        public readonly Term[] Terms;

        internal SumTerm(Term[] terms) =>
            this.Terms = terms;

        public override Term HigherOrder =>
            new SumTerm(this.Terms.Select(term => term.HigherOrder).ToArray());

        public override Term Reduce(ReduceContext context)
        {
            var terms = this.Terms.Aggregate(
                Enumerable.Empty<Term>(),
                (agg, term) => term is SumTerm sum ?
                    agg.Concat(sum.Terms) :
                    agg.Concat(new[] { term })).
                ToArray();

            Debug.Assert(terms.Length >= 1);
            return new SumTerm(terms);
        }

        public override Term Infer(InferContext context) =>
            new SumTerm(this.Terms.Select(term => term.Infer(context)).ToArray());

        public override Term Fixup(FixupContext context) =>
            new SumTerm(this.Terms.Select(term => term.Fixup(context)).ToArray());

        public override bool Equals(Term? other) =>
            other is SumTerm rhs ? rhs.Terms.SequenceEqual(this.Terms) : false;

        public override int GetHashCode() =>
            this.Terms.Aggregate(0, (agg, term) => agg ^ term.GetHashCode());

        public void Deconstruct(out Term[] terms) =>
            terms = this.Terms;
    }
}
