using Favalon.Terms;
using System.Collections.Generic;

namespace Favalon.Contexts
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
}
