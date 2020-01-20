using Favalon.Terms;
using System;
using System.Collections.Generic;

namespace Favalon.Contexts
{
    public abstract class Context
    {
        internal sealed class PlaceholderIndexer
        {
            private int current;

            public PlaceholderTerm Create(Term higherOrder) =>
                new PlaceholderTerm(current++, higherOrder);
        }

        internal readonly PlaceholderIndexer indexer;
        internal readonly Context? parent;
        internal Dictionary<string, Term>? boundTerms;

        private protected Context()
        {
            indexer = new PlaceholderIndexer();
            boundTerms = new Dictionary<string, Term>();
        }

        private protected Context(Context parent)
        {
            this.indexer = parent.indexer;
            this.parent = parent;
        }

        internal Context(Context parent, Dictionary<string, Term> boundTerms)
        {
            this.indexer = parent.indexer;
            this.parent = parent;
            this.boundTerms = boundTerms;
        }

        /////////////////////////////////////////////////////////////////////////
        // Binder

        public void SetBoundTerm(string identity, Term term)
        {
            if (boundTerms == null)
            {
                boundTerms = new Dictionary<string, Term>();
            }

            boundTerms[identity] = term;
        }

        public Term? LookupBoundTerm(string identity)
        {
            Context? current = this;
            do
            {
                if (current.boundTerms != null)
                {
                    if (current.boundTerms.TryGetValue(identity, out var term))
                    {
                        return term;
                    }
                }
                current = current.parent;
            }
            while (current != null);

            return null;
        }

        /////////////////////////////////////////////////////////////////////////
        // Infer

        private protected Term InternalInfer(Term term, Dictionary<string, Term> boundTerms)
        {
            var context = new InferContext(this, boundTerms);
            var partial = term.Infer(context);
            return partial.Fixup(context);
        }

        private protected Term InternalInfer(Term term) =>
            this.InternalInfer(
                term,
                this.boundTerms is Dictionary<string, Term> boundTerms ?
                    new Dictionary<string, Term>(boundTerms) : // Copied, eliminate side effects by BindTerm
                    new Dictionary<string, Term>());

        /////////////////////////////////////////////////////////////////////////
        // Reduce

        private protected Term InternalReduce(Term term, Dictionary<string, Term> boundTerms, int iterations)
        {
            var context = new ReduceContext(this, boundTerms, iterations);
            return term.Reduce(context);
        }

        private protected IEnumerable<Term> InternalEnumerableReduce(Term term, int iterations)
        {
            var boundTerms =
                this.boundTerms is Dictionary<string, Term> bt ?
                    new Dictionary<string, Term>(bt) : // Copied, eliminate side effects by BindTerm
                    new Dictionary<string, Term>();

            var current = term;
            var iteration = 0;
            for (; iteration < iterations; iteration++)
            {
                yield return current;

                var inferred = this.InternalInfer(current, boundTerms);
                if (!current.EqualsWithHigherOrder(inferred))
                {
                    yield return inferred;
                }

                var reduced = this.InternalReduce(inferred, boundTerms, iterations);
                if (current.EqualsWithHigherOrder(reduced))
                {
                    break;
                }

                current = reduced;
            }

            if (iteration >= iterations)
            {
                // TODO: Detects uninterpretable terms on many iterations.
                throw new InvalidOperationException();
            }

            // Applied if wasn't caused exceptions.
            if (boundTerms != null)
            {
                if (this.boundTerms != null)
                {
                    // Apply finally bound result.
                    foreach (var entry in boundTerms)
                    {
                        this.boundTerms[entry.Key] = entry.Value;
                    }
                }
                else
                {
                    this.boundTerms = boundTerms;
                }
            }
        }
    }
}
