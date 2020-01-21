﻿using Favalon.Terms;
using System;
using System.Collections.Generic;

namespace Favalon.TermContexts
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
        internal Dictionary<string, Term>? boundTerms
        {
            get;
            private set;
        }

        public readonly int Iterations;

        private protected Context(int iterations)
        {
            indexer = new PlaceholderIndexer();
            this.Iterations = iterations;
        }

        private protected Context(Context parent)
        {
            this.indexer = parent.indexer;
            this.parent = parent;
            this.Iterations = parent.Iterations;
        }

        private protected Context(Context parent, Dictionary<string, Term> boundTerms)
        {
            this.indexer = parent.indexer;
            this.parent = parent;
            this.boundTerms = boundTerms;
            this.Iterations = parent.Iterations;
        }

        /////////////////////////////////////////////////////////////////////////
        // Binder

        public void SetBindTerm(string identity, Term term)
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

        private Term InternalInfer(Term term, Dictionary<string, Term> boundTerms, bool higherOrderInferOnly)
        {
            var context = new InferContext(this, boundTerms, higherOrderInferOnly);
            var partial = term.Infer(context);
            return partial.Fixup(context);
        }

        private protected IEnumerable<Term> InternalEnumerableInfer(Term term, bool higherOrderInferOnly)
        {
            var boundTerms =
                this.boundTerms is Dictionary<string, Term> bt ?
                    new Dictionary<string, Term>(bt) : // Copied, eliminate side effects by BindTerm
                    new Dictionary<string, Term>();

            var current = term;
            var iteration = 0;
            for (; iteration < this.Iterations; iteration++)
            {
                yield return current;

                var inferred = this.InternalInfer(current, boundTerms, higherOrderInferOnly);
                if (current.EqualsWithHigherOrder(inferred))
                {
                    break;
                }

                current = inferred;
            }

            if (iteration >= this.Iterations)
            {
                yield return current;

                // TODO: Detects uninterpretable terms on many iterations.
                throw new InvalidOperationException();
            }
        }

        /////////////////////////////////////////////////////////////////////////
        // Reduce

        private protected Term InternalReduce(Term term, Dictionary<string, Term> boundTerms)
        {
            var context = new ReduceContext(this, boundTerms);
            return term.Reduce(context);
        }

        private protected IEnumerable<Term> InternalEnumerableReduce(Term term)
        {
            var boundTerms =
                this.boundTerms is Dictionary<string, Term> bt ?
                    new Dictionary<string, Term>(bt) : // Copied, eliminate side effects by BindTerm
                    new Dictionary<string, Term>();

            var current = term;
            var iteration = 0;
            for (; iteration < this.Iterations; iteration++)
            {
                yield return current;

                var inferred = this.InternalInfer(current, boundTerms, false);
                if (!current.EqualsWithHigherOrder(inferred))
                {
                    yield return inferred;
                }

                var reduced = this.InternalReduce(inferred, boundTerms);
                if (inferred.EqualsWithHigherOrder(reduced))
                {
                    break;
                }

                current = reduced;
            }

            if (iteration >= this.Iterations)
            {
                yield return current;

                // TODO: Detects uninterpretable terms on many iterations.
                throw new InvalidOperationException();
            }

            // Applied if wasn't caused exceptions.
            this.boundTerms = boundTerms;
        }
    }
}
