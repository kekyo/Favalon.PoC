using Favalon.Terms;
using System.Collections.Generic;

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

        private UnifyResult Unify(PlaceholderTerm placeholder, Term term)
        {
            if (placeholders.TryGetValue(placeholder.Index, out var last))
            {
                return this.Unify(last, term);
            }
            else
            {
                placeholders.Add(placeholder.Index, term);
                return new UnifyResult(true, term);
            }
        }

        // Basically term2 will replace term1, but also swaps when be included PlaceHolderTerm.
        public UnifyResult Unify(Term term1, Term term2)
        {
            if (object.ReferenceEquals(term1, term2) || term1.Equals(term2))
            {
                return new UnifyResult(true, term1);
            }
            else if (term1 is PlaceholderTerm placeholder1)
            {
                return this.Unify(placeholder1, term2);
            }
            else if (term2 is PlaceholderTerm placeholder2)
            {
                return this.Unify(placeholder2, term1);
            }
            else if
                (term1 is LambdaTerm(Term parameter1, Term body1) &&
                 term2 is LambdaTerm(Term parameter2, Term body2))
            {
                var (parameterUnified, parameterTerm) = this.Unify(parameter1, parameter2);
                var (bodyUnified, bodyTerm) = this.Unify(body1, body2);

                return new UnifyResult(
                    parameterUnified && bodyUnified,
                    LambdaTerm.Create(parameterTerm, bodyTerm));
            }
            else
            {
                // Higher priority term2 rather than term1.
                // (The terms unmarked nullable-refs in C#, but also come from 4th order or Unspecified higher order 8)
                return new UnifyResult(false, term2 ?? term1);
            }
        }
    }
}
