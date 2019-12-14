using Favalon.Contexts;

namespace Favalon.Terms.AlgebricData
{
    public sealed class AndTerm : MultipleTerm<AndTerm>
    {
        internal AndTerm(Term[] terms) :
            base(terms)
        { }

        protected override Term Create(Term[] terms) =>
            new AndTerm(terms);
    }
}
