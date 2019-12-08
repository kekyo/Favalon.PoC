using System.Diagnostics;
using System.Linq;

namespace Favalon.AlgebricData
{
    public abstract class MultipleTerm : HigherOrderLazyTerm
    {
        public readonly Term[] Terms;

        private protected MultipleTerm(Term[] terms) =>
            this.Terms = terms;

        protected override sealed Term GetHigherOrder() =>
            this.Create(this.Terms.Select(term => term.HigherOrder).ToArray());

        protected abstract Term Create(Term[] terms);

        public override sealed Term Infer(InferContext context)
        {
            var terms = this.Terms.Select(term => term.Infer(context)).ToArray();

            Debug.Assert(terms.Length >= 1);
            return terms.Zip(this.Terms, object.ReferenceEquals).All(r => r) ?
                this :
                this.Create(terms);
        }

        public override sealed Term Fixup(FixupContext context)
        {
            var terms = this.Terms.Select(term => term.Fixup(context)).ToArray();

            Debug.Assert(terms.Length >= 1);
            return terms.Zip(this.Terms, object.ReferenceEquals).All(r => r) ?
                this :
                this.Create(terms);
        }

        public override sealed Term Reduce(ReduceContext context)
        {
            var terms = this.Terms.Select(term => term.Reduce(context)).ToArray();

            Debug.Assert(terms.Length >= 1);
            return terms.Zip(this.Terms, object.ReferenceEquals).All(r => r) ?
                this :
                this.Create(terms);
        }

        public override sealed int GetHashCode() =>
            this.Terms.Aggregate(0, (agg, term) => agg ^ term.GetHashCode());

        public void Deconstruct(out Term[] terms) =>
            terms = this.Terms;
    }

    public abstract class MultipleTerm<T> : MultipleTerm
        where T : MultipleTerm
    {
        private protected MultipleTerm(Term[] terms) :
            base(terms)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T rhs ? rhs.Terms.SequenceEqual(this.Terms) : false;
    }
}
