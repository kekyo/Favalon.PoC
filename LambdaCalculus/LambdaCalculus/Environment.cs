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

        public static Environment Create(int defaultIterations = 10000) =>
            new Environment(defaultIterations);

        /////////////////////////////////////////////////////////////////////////
        // Infer

        public IEnumerable<Term> EnumerableInfer(
            Term term,
            int iterations,
            bool higherOrderInferOnly) =>
            base.InternalEnumerableInfer(term, higherOrderInferOnly, iterations);

        public IEnumerable<Term> EnumerableInfer(Term term) =>
            base.InternalEnumerableInfer(term, false, DefaultIterations);

        public Term Infer(
            Term term,
            int iterations,
            bool higherOrderInferOnly) =>
            base.InternalEnumerableInfer(term, higherOrderInferOnly, iterations).Last();

        public Term Infer(Term term) =>
            base.InternalEnumerableInfer(term, false, DefaultIterations).Last();

        public Term InferOne(
            Term term,
            int iterations,
            bool higherOrderInferOnly) =>
            base.InternalEnumerableInfer(term, higherOrderInferOnly, iterations).First();

        public Term InferOne(Term term) =>
            base.InternalEnumerableInfer(term, false, DefaultIterations).First();

        /////////////////////////////////////////////////////////////////////////
        // Reduce

        public IEnumerable<Term> EnumerableReduce(Term term, int iterations) =>
            base.InternalEnumerableReduce(term, iterations);

        public IEnumerable<Term> EnumerableReduce(Term term) =>
            base.InternalEnumerableReduce(term, DefaultIterations);

        public Term Reduce(Term term, int iterations) =>
            base.InternalEnumerableReduce(term, iterations).Last();

        public Term Reduce(Term term) =>
            base.InternalEnumerableReduce(term, DefaultIterations).Last();

        public Term ReduceOne(Term term, int iterations) =>
            base.InternalEnumerableReduce(term, iterations).First();

        public Term ReduceOne(Term term) =>
            base.InternalEnumerableReduce(term, DefaultIterations).First();
    }
}
