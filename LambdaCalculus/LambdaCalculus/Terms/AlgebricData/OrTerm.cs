namespace Favalon.Terms.AlgebricData
{
    public sealed class OrTerm : MultipleTerm<OrTerm>
    {
        internal OrTerm(Term[] terms) :
            base(terms)
        { }

        protected override Term Create(Term[] terms) =>
            new OrTerm(terms);
    }
}
