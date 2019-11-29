using System;
using System.Linq;

namespace Favalon.Terms
{
    public sealed class IdentityTerm :
        VariableTerm
    {
        internal IdentityTerm(string identity) :
            base(identity)
        { }

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            (identity == this.Name) ?
                replacement :
                this;

        protected internal override Term VisitReduce(Context context) =>
            context.LookupBoundTerms(this.Name) is BoundTermInformation[] terms ?
                terms[0].Term :
                this;

        protected internal override Term[] VisitInfer(Context context) =>
            context.LookupBoundTerms(this.Name) is BoundTermInformation[] terms ?
                terms.Select(term => term.Term).ToArray() :
                new[] { this };

        public BoundIdentityTerm ToBoundIdentity() =>
            new BoundIdentityTerm(this.Name);
    }
}
