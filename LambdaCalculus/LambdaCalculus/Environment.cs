using System.Collections.Generic;
using System.Diagnostics;

namespace LambdaCalculus
{
    public class Environment
    {
        internal sealed class PlaceholderIndexer
        {
            private int current;

            public PlaceholderTerm Create(Term higherOrder) =>
                new PlaceholderTerm(current++, higherOrder);
        }

        internal readonly PlaceholderIndexer indexer;
        internal readonly Environment? parent;
        internal Dictionary<string, Term>? boundTerms;

        private Environment()
        {
            indexer = new PlaceholderIndexer();
            boundTerms = new Dictionary<string, Term>();
        }

        internal Environment(Environment parent)
        {
            this.indexer = parent.indexer;
            this.parent = parent;
        }

        public void AddBoundTerm(string identity, Term term)
        {
            if (boundTerms == null)
            {
                boundTerms = new Dictionary<string, Term>();
            }
            boundTerms[identity] = term;
        }

        public Term Reduce(Term term)
        {
            var context = new ReduceContext(this);
            return term.Reduce(context);
        }

        public Term Infer(Term term)
        {
            var context = new InferContext(this);
            var inferred = term.Infer(context);
            return inferred.Fixup(context);
        }

        public Term? GetBoundTerm(string identity)
        {
            Environment? current = this;
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

        public static Environment Create() =>
            new Environment();
    }

    public sealed class ReduceContext : Environment
    {
        internal ReduceContext(Environment parent) :
            base(parent)
        { }

        public ReduceContext NewScope() =>
            new ReduceContext(this);
    }

    public sealed class InferContext : Environment
    {
        private readonly Dictionary<int, Term> placeholders;

        internal InferContext(Environment parent) :
            base(parent) =>
            placeholders = new Dictionary<int, Term>();

        private InferContext(Environment parent, Dictionary<int, Term> placeholders) :
            base(parent) =>
            this.placeholders = placeholders;

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

            if (term1 is PlaceholderTerm placeholder1)
            {
                Unify(placeholder1, term2);
            }
            else if (term2 is PlaceholderTerm placeholder2)
            {
                Unify(placeholder2, term1);
            }
        }

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
}
