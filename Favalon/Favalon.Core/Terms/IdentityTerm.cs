using System;

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

        protected internal override sealed Term VisitReduce(Context context) =>
            context.LookupBoundTerms(this.Name) is BoundTermInformation[] terms ?
                terms[0].Term :
                this;

        public BoundIdentityTerm ToBoundIdentity() =>
            new BoundIdentityTerm(this.Name);
    }
}
