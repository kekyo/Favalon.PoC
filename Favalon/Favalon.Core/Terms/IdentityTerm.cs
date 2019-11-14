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
    }
}
