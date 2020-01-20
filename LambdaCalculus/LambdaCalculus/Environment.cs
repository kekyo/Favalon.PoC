﻿using Favalon.Contexts;
using Favalon.Terms;
using System.Collections.Generic;

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
            var context = new InferContext(this);
            var partial = term.Infer(context);
            return partial.Fixup(context);
        }

        public IEnumerable<Term> EnumerableReduce(Term term) =>
            this.EnumerableReduce(term, DefaultIterations);

        public IEnumerable<Term> EnumerableReduce(Term term, int iterations)
        {
            if (boundTerms == null)
            {
                boundTerms = new Dictionary<string, Term>();
            }

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

                var context = new ReduceContext(this, boundTerms, iterations);
                var reduced = inferred.Reduce(context);

                if (object.ReferenceEquals(reduced, current))
                {
                    break;
                }

                current = reduced;
            }

            // TODO: Detects uninterpretable terms on many iterations.
        }

        public Term ReduceOne(Term term) =>
            this.ReduceOne(term, DefaultIterations);

        public Term ReduceOne(Term term, int iterations)
        {
            if (boundTerms == null)
            {
                boundTerms = new Dictionary<string, Term>();
            }

            var inferred = this.Infer(term);

            var context = new ReduceContext(this, boundTerms, iterations);
            return inferred.Reduce(context);
        }

        public Term Reduce(Term term) =>
            this.Reduce(term, DefaultIterations);

        public Term Reduce(Term term, int iterations)
        {
            if (boundTerms == null)
            {
                boundTerms = new Dictionary<string, Term>();
            }

            var current = term;
            for (var iteration = 0; iteration < iterations; iteration++)
            {
                var inferred = this.Infer(current);

                if (object.ReferenceEquals(inferred, current))
                {
                    break;
                }

                var context = new ReduceContext(this, boundTerms, iterations);
                var reduced = inferred.Reduce(context);

                if (object.ReferenceEquals(reduced, current))
                {
                    break;
                }

                current = reduced;
            }

            // TODO: Detects uninterpretable terms on many iterations.

            return current;
        }

        public static Environment Create(int defaultIterations = 10000) =>
            new Environment(defaultIterations);
    }
}
