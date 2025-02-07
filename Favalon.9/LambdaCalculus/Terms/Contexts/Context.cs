﻿using System;
using System.Collections.Generic;
using Favalon.Terms.Types;

namespace Favalon.Terms.Contexts
{
    public abstract class Context
    {
        internal sealed class PlaceholderIndexer
        {
            private int current;

            public PlaceholderTerm Create(Term higherOrder) =>
                new PlaceholderTerm(current++, higherOrder);
        }

        internal readonly TypeCalculator calculator;
        internal readonly PlaceholderIndexer indexer;
        internal readonly Context? parent;

        private Dictionary<string, Term>? boundTerms;

        public readonly int Iterations;

        private protected Context(int iterations, TypeCalculator calculator)
        {
            indexer = new PlaceholderIndexer();
            this.calculator = calculator;
            this.Iterations = iterations;
        }

        private protected Context(Context parent)
        {
            this.indexer = parent.indexer;
            this.calculator = parent.calculator;
            this.parent = parent;
            this.Iterations = parent.Iterations;
        }

        private protected Context(Context parent, Dictionary<string, Term> boundTerms)
        {
            this.indexer = parent.indexer;
            this.calculator = parent.calculator;
            this.parent = parent;
            this.boundTerms = boundTerms;
            this.Iterations = parent.Iterations;
        }

        /////////////////////////////////////////////////////////////////////////
        // Binder

        public void BindMutable(string identity, Term term)
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

        private void applyBoundTerms(Dictionary<string, Term> boundTerms)
        {
            if (this.boundTerms != null)
            {
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
            var boundTerms = new Dictionary<string, Term>();
            var current = term;
            var iteration = 0;
            while (true)
            {
                yield return current;

                if (iteration >= this.Iterations)
                {
                    // TODO: Detects uninterpretable terms on many iterations.
                    throw new InvalidOperationException();
                }

                var inferred = this.InternalInfer(current, boundTerms, higherOrderInferOnly);
                if (current.EqualsWithHigherOrder(inferred))
                {
                    break;
                }

                current = inferred;
                iteration++;
            }

            // Inferring with reducing will side effect bounding terms.
            if (!higherOrderInferOnly)
            {
                // Applied bound terms if wasn't caused exceptions.
                this.applyBoundTerms(boundTerms);
            }
        }

        /////////////////////////////////////////////////////////////////////////
        // Reduce

        private Term InternalReduce(Term term, Dictionary<string, Term> boundTerms)
        {
            var context = new ReduceContext(this, boundTerms);
            return term.Reduce(context);
        }

        private protected IEnumerable<Term> InternalEnumerableReduce(Term term)
        {
            var boundTerms = new Dictionary<string, Term>();
            var current = term;
            var iteration = 0;
            while (true)
            {
                yield return current;

                if (iteration >= this.Iterations)
                {
                    // TODO: Detects uninterpretable terms on many iterations.
                    throw new InvalidOperationException();
                }

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
                iteration++;
            }

            // Applied bound terms if wasn't caused exceptions.
            this.applyBoundTerms(boundTerms);
        }
    }
}
