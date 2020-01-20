using Favalon.Contexts;
using Favalon.Terms;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    public class Environment : Context
    {
        public readonly int DefaultIterations;

        protected Environment(int defaultIterations)
        {
            this.DefaultIterations = defaultIterations;
        }

        public PlaceholderTerm CreatePlaceholder(Term higherOrder) =>
            indexer.Create(higherOrder);

        public Term Infer(Term term)
        {
            var temporaryBoundTerms =
                this.boundTerms is Dictionary<string, Term> boundTerms ?
                    new Dictionary<string, Term>(boundTerms) : // Copied, eliminate side effects by BindTerm
                    new Dictionary<string, Term>();

            var context = new InferContext(this, temporaryBoundTerms);
            var partial = term.Infer(context);
            return partial.Fixup(context);
        }

        public IEnumerable<Term> EnumerableReduce(Term term) =>
            this.EnumerableReduce(term, DefaultIterations);

        public IEnumerable<Term> EnumerableReduce(Term term, int iterations)
        {
            Dictionary<string, Term>? temporaryBoundTerms = null;

            var current = term;
            for (var iteration = 0; iteration < iterations; iteration++)
            {
                yield return current;

                var inferred = this.Infer(current);

                if (object.ReferenceEquals(inferred, current))
                {
                    break;
                }

                yield return inferred;

                temporaryBoundTerms =
                    this.boundTerms is Dictionary<string, Term> boundTerms ?
                        new Dictionary<string, Term>(boundTerms) : // Copied, eliminate side effects by BindTerm
                        new Dictionary<string, Term>();

                var context = new ReduceContext(this, temporaryBoundTerms, iterations);
                var reduced = inferred.Reduce(context);

                if (object.ReferenceEquals(reduced, current))
                {
                    break;
                }

                current = reduced;
            }

            // Applied if didn't cause exceptions.
            if (temporaryBoundTerms != null)
            {
                if (this.boundTerms != null)
                {
                    // Apply finally bound result.
                    foreach (var entry in temporaryBoundTerms)
                    {
                        this.boundTerms[entry.Key] = entry.Value;
                    }
                }
                else
                {
                    this.boundTerms = temporaryBoundTerms;
                }
            }

            // TODO: Detects uninterpretable terms on many iterations.
        }

        public Term ReduceOne(Term term) =>
            this.ReduceOne(term, DefaultIterations);

        public Term ReduceOne(Term term, int iterations)
        {
            var inferred = this.Infer(term);

            var temporaryBoundTerms =
                this.boundTerms is Dictionary<string, Term> boundTerms ?
                    new Dictionary<string, Term>(boundTerms) : // Copied, eliminate side effects by BindTerm
                    new Dictionary<string, Term>();

            var context = new ReduceContext(this, temporaryBoundTerms, iterations);
            var reduced = inferred.Reduce(context);

            // Applied if didn't cause exceptions.
            this.boundTerms = temporaryBoundTerms;

            return reduced;
        }

        public Term Reduce(Term term) =>
            this.Reduce(term, DefaultIterations);

        public Term Reduce(Term term, int iterations) =>
            this.EnumerableReduce(term, iterations).Last();

        public static Environment Create(int defaultIterations = 10000) =>
            new Environment(defaultIterations);
    }
}
