using Favalon.Terms;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Contexts
{
    public sealed class InferContext : FixupContext
    {
        private readonly bool higherOrderInferOnly;
        private readonly int iterations;

        internal InferContext(
            Context parent,
            Dictionary<string, Term> boundTerms,
            bool higherOrderInferOnly,
            int iterations) :
            base(parent, boundTerms, new Dictionary<int, Term>())
        {
            this.higherOrderInferOnly = higherOrderInferOnly;
            this.iterations = iterations;
        }

        private InferContext(
            InferContext parent,
            Dictionary<int, Term> placeholders) :
            base(parent, new Dictionary<string, Term>(), placeholders)
        {
            this.higherOrderInferOnly = parent.higherOrderInferOnly;
            this.iterations = parent.iterations;
        }

        public InferContext NewScope() =>
            new InferContext(this, placeholders);

        public PlaceholderTerm CreatePlaceholder(Term higherOrder) =>
            indexer.Create(higherOrder);

        public Term ResolveHigherOrder(Term higherOrder) =>
            higherOrderInferOnly ?
                higherOrder.Infer(this) :
                base.InternalEnumerableReduce(higherOrder, iterations).Last();

        /////////////////////////////////////////////////////////////////////////
        // Unify

        private bool Unify(PlaceholderTerm placeholder, Term term)
        {
            if (placeholders.TryGetValue(placeholder.Index, out var last))
            {
                return Unify(last, term);
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

            if (term1 is TerminationTerm || term2 is TerminationTerm)
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
                unified = Unify(placeholder1, term2);
            }
            else if (term2 is PlaceholderTerm placeholder2)
            {
                unified = Unify(placeholder2, term1);
            }
            else if (term1 is LambdaTerm(Term parameter1, Term body1) &&
                term2 is LambdaTerm(Term parameter2, Term body2))
            {
                var unified1 = Unify(parameter1, parameter2);
                var unified2 = Unify(body1, body2);
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
