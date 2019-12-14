using Favalon.Terms;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Contexts
{
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
