using Favalon.Contexts;
using Favalon.Terms;
using System;
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

        public static Environment Create(int defaultIterations = 10000) =>
            new Environment(defaultIterations);

        public Term Infer(Term term) =>
            base.InternalInfer(term);

        public IEnumerable<Term> EnumerableReduce(Term term, int iterations) =>
            base.InternalEnumerableReduce(term, iterations);

        public IEnumerable<Term> EnumerableReduce(Term term) =>
            base.InternalEnumerableReduce(term, DefaultIterations);

        public Term ReduceOne(Term term, int iterations)
        {
            var boundTerms =
                this.boundTerms is Dictionary<string, Term> bt ?
                    new Dictionary<string, Term>(bt) : // Copied, eliminate side effects by BindTerm
                    new Dictionary<string, Term>();

            var inferred = base.InternalInfer(term, boundTerms);
            var reduced = base.InternalReduce(inferred, boundTerms, iterations);

            // Applied if wasn't caused exceptions.
            this.boundTerms = boundTerms;

            return reduced;
        }

        public Term ReduceOne(Term term) =>
            this.ReduceOne(term, DefaultIterations);

        public Term Reduce(Term term, int iterations) =>
            this.EnumerableReduce(term, iterations).Last();

        public Term Reduce(Term term) =>
            this.Reduce(term, DefaultIterations);
    }
}
