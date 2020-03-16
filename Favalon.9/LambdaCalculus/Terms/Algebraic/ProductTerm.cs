using Favalon.Terms.Contexts;
using System;
using System.Linq;

namespace Favalon.Terms.Algebraic
{
    public sealed class ProductTerm : AlgebraicTerm<ProductTerm>
    {
        private ProductTerm(Term[] terms, Term higherOrder) :
            base(terms, higherOrder)
        { }

        protected override Term OnCreate(Term[] terms, Term higherOrder) =>
            new ProductTerm(terms, higherOrder);

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            Utilities.Join(" * ", this.Terms.Select(term => term.PrettyPrint(context)));

        public static ProductTerm Create(Term[] terms, Term higherOrder) =>
            new ProductTerm(terms, higherOrder);

        public static Term? From(Term[] terms, Term higherOrder) =>
            terms.Length switch
            {
                0 => null,
                1 => terms[0],
                _ => new ProductTerm(terms, higherOrder)
            };
    }
}
