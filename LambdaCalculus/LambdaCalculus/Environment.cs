using Favalon.TermContexts;
using Favalon.Terms;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    public class Environment : Context
    {
        protected const int DefaultIterations = 1000;

        protected Environment(int iterations) :
            base(iterations)
        { }

        public PlaceholderTerm CreatePlaceholder(Term higherOrder) =>
            indexer.Create(higherOrder);

        public static Environment Create(int defaultIterations = DefaultIterations) =>
            new Environment(defaultIterations);

        /////////////////////////////////////////////////////////////////////////
        // Infer

        public IEnumerable<Term> EnumerableInfer(Term term, bool higherOrderInferOnly) =>
            base.InternalEnumerableInfer(term, higherOrderInferOnly);

        public IEnumerable<Term> EnumerableInfer(Term term) =>
            base.InternalEnumerableInfer(term, false);

        public Term Infer(Term term, bool higherOrderInferOnly) =>
            base.InternalEnumerableInfer(term, higherOrderInferOnly).Last();

        public Term Infer(Term term) =>
            base.InternalEnumerableInfer(term, false).Last();

        public Term InferOne(Term term, bool higherOrderInferOnly) =>
            base.InternalEnumerableInfer(term, higherOrderInferOnly).First();

        public Term InferOne(Term term) =>
            base.InternalEnumerableInfer(term, false).First();

        /////////////////////////////////////////////////////////////////////////
        // Reduce

        public IEnumerable<Term> EnumerableReduce(Term term) =>
            base.InternalEnumerableReduce(term);

        public Term Reduce(Term term) =>
            base.InternalEnumerableReduce(term).Last();

        public Term ReduceOne(Term term) =>
            base.InternalEnumerableReduce(term).First();
    }
}
