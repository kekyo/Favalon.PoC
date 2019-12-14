﻿using Favalon.Terms;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    public class Context
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
    }

    public sealed class Environment : Context
    {
        public Term Infer(Term term)
        {
            var context = new InferContext(this);
            var partial = term.Infer(context);
            return partial.Fixup(context);
        }

        public IEnumerable<Term> EnumerableReduce(Term term)
        {
            var inferred = this.Infer(term);

            if (boundTerms == null)
            {
                boundTerms = new Dictionary<string, Term>();
            }

            var current = inferred;
            while (true)
            {
                yield return current;

                var context = new ReduceContext(this, boundTerms);

                var reduced = current.Reduce(context);
                if (object.ReferenceEquals(reduced, current))
                {
                    break;
                }

                current = reduced;
            }
        }

        public Term Reduce(Term term) =>
            this.EnumerableReduce(term).Last();

        public static Environment Create() =>
            new Environment();
    }

    public sealed class ReduceContext : Context
    {
        internal ReduceContext(Context parent) :
            base(parent)
        { }

        internal ReduceContext(
            Context parent,
            Dictionary<string, Term> boundTerms) :
            base(parent, boundTerms)
        { }

        public ReduceContext NewScope() =>
            new ReduceContext(this);
    }

    public class FixupContext : Context
    {
        private protected readonly Dictionary<int, Term> placeholders;

        private protected FixupContext(Context parent, Dictionary<int, Term> placeholders) :
            base(parent) =>
            this.placeholders = placeholders;

        public Term LookupUnifiedTerm(PlaceholderTerm placeholder)
        {
            var current = placeholder;
            while (true)
            {
                if (placeholders.TryGetValue(current.Index, out var next))
                {
                    if (next is PlaceholderTerm p)
                    {
                        current = p;
                    }
                    else
                    {
                        return next;
                    }
                }
                else
                {
                    return current;
                }
            }
        }
    }

    public sealed class InferContext : FixupContext
    {
        internal InferContext(Context parent) :
            base(parent, new Dictionary<int, Term>())
        { }

        private InferContext(Context parent, Dictionary<int, Term> placeholders) :
            base(parent, placeholders)
        { }

        public InferContext NewScope() =>
            new InferContext(this, placeholders);

        public PlaceholderTerm CreatePlaceholder(Term higherOrder) =>
            indexer.Create(higherOrder);

        private void Unify(PlaceholderTerm placeholder, Term term)
        {
            if (placeholders.TryGetValue(placeholder.Index, out var last))
            {
                Unify(last, term);
            }
            else
            {
                placeholders.Add(placeholder.Index, term);
            }
        }

        public void Unify(Term term1, Term term2)
        {
            if (object.ReferenceEquals(term1, term2) || term1.Equals(term2))
            {
                return;
            }

            if (term1 is LambdaTerm(Term parameter1, Term body1) &&
                term2 is LambdaTerm(Term parameter2, Term body2))
            {
                Unify(parameter1, parameter2);
                Unify(body1, body2);
            }
            else if (term1 is PlaceholderTerm placeholder1)
            {
                Unify(placeholder1, term2);
            }
            else if (term2 is PlaceholderTerm placeholder2)
            {
                Unify(placeholder2, term1);
            }
        }
    }
}
