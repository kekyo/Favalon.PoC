using Favalon.Terms;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms.Contexts
{
    public sealed class InferContext : FixupContext
    {
        private readonly bool higherOrderInferOnly;

        internal InferContext(
            Context parent,
            Dictionary<string, Term> boundTerms,
            bool higherOrderInferOnly) :
            base(parent, boundTerms, new Dictionary<int, Term>()) =>
            this.higherOrderInferOnly = higherOrderInferOnly;

        private InferContext(
            InferContext parent,
            Dictionary<int, Term> placeholders) :
            base(parent, new Dictionary<string, Term>(), placeholders) =>
            this.higherOrderInferOnly = parent.higherOrderInferOnly;

        public InferContext NewScope() =>
            new InferContext(this, placeholders);

        public PlaceholderTerm CreatePlaceholder(Term higherOrder) =>
            indexer.Create(higherOrder);

        public Term ResolveHigherOrder(Term term) =>
            higherOrderInferOnly ?
                term.HigherOrder.Infer(this) :
                base.InternalEnumerableReduce(term.HigherOrder).Last();

        /////////////////////////////////////////////////////////////////////////
        // Unify

        private bool Unify(PlaceholderTerm placeholder, Term term)
        {
            if (placeholders.TryGetValue(placeholder.Index, out var last))
            {
                return this.Unify(last, term);
            }
            else
            {
                placeholders.Add(placeholder.Index, term);
                return true;
            }
        }

        public bool Unify(Term term1, Term term2)
        {
            if (term1 == null || term2 == null)
            {
                return false;
            }

            if (!term1.ValidTerm || !term2.ValidTerm)
            {
                return false;
            }

            if (term1.EqualsWithHigherOrder(term2))
            {
                return true;
            }

            bool unified;

            if (term1 is PlaceholderTerm placeholder1)
            {
                unified = this.Unify(placeholder1, term2);
            }
            else if (term2 is PlaceholderTerm placeholder2)
            {
                unified = this.Unify(placeholder2, term1);
            }
            else if (term1 is LambdaTerm(Term parameter1, Term body1) &&
                term2 is LambdaTerm(Term parameter2, Term body2))
            {
                var unified1 = this.Unify(parameter1, parameter2);
                var unified2 = this.Unify(body1, body2);
                unified = unified1 && unified2;
            }
            else
            {
                unified = false;
            }

            // Unify higher orders.
            this.Unify(term1.HigherOrder, term2.HigherOrder);

            return unified;
        }
    }
}
