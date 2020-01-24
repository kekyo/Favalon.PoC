using Favalon.Terms.Contexts;
using Favalon.Terms;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Favalon
{
    public sealed class Environment : Context
    {
        private Environment(int iterations) :
            base(iterations)
        { }

        public PlaceholderTerm CreatePlaceholder(Term higherOrder) =>
            indexer.Create(higherOrder);

        public static Environment Pure(int iterations = EnvironmentFactory.DefaultIterations) =>
            new Environment(iterations);

        /////////////////////////////////////////////////////////////////////////
        // Binder

        public new Environment BindTerm(string identity, Term term)
        {
            base.BindTerm(identity, term);
            return this;
        }

        /////////////////////////////////////////////////////////////////////////
        // Infer

        public IEnumerable<Term> EnumerableInfer(Term term, bool higherOrderInferOnly) =>
            base.InternalEnumerableInfer(term, higherOrderInferOnly);

        public IEnumerable<Term> EnumerableInfer(Term term) =>
            base.InternalEnumerableInfer(term, false);

#if DEBUG
        public Term Infer(Term term, bool higherOrderInferOnly)
        {
            Term? result = null;
            foreach (var inferred in base.InternalEnumerableInfer(term, higherOrderInferOnly))
            {
                result = inferred;
                Debug.WriteLine(inferred.Full);
            }
            return result!;
        }
#else
        public Term Infer(Term term, bool higherOrderInferOnly) =>
            base.InternalEnumerableInfer(term, higherOrderInferOnly).Last();
#endif

        public Term Infer(Term term) =>
            this.Infer(term, false);

        public Term InferOne(Term term, bool higherOrderInferOnly) =>
            base.InternalEnumerableInfer(term, higherOrderInferOnly).First();

        public Term InferOne(Term term) =>
            base.InternalEnumerableInfer(term, false).First();

        /////////////////////////////////////////////////////////////////////////
        // Reduce

        public IEnumerable<Term> EnumerableReduce(Term term) =>
            base.InternalEnumerableReduce(term);

#if DEBUG
        public Term Reduce(Term term)
        {
            Term? result = null;
            foreach (var reduced in InternalEnumerableReduce(term))
            {
                result = reduced;
                Debug.WriteLine(reduced.Full);
            }
            return result!;
        }
#else
        public Term Reduce(Term term) =>
            base.InternalEnumerableReduce(term).Last();
#endif

        public Term ReduceOne(Term term) =>
            base.InternalEnumerableReduce(term).First();
    }
}
