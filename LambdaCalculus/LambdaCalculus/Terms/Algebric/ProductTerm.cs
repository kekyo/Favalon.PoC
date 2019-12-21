namespace Favalon.Terms.Algebric
{
    public sealed class ProductTerm : MultipleTerm<ProductTerm>
    {
        internal ProductTerm(Term[] terms) :
            base(terms)
        { }

        protected override Term Create(Term[] terms) =>
            new ProductTerm(terms);
    }
}
