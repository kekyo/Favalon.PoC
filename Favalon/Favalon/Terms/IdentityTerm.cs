using System;

namespace Favalon.Terms
{
    public sealed class IdentityTerm :
        Term, IEquatable<IdentityTerm?>
    {
        public new readonly string Identity;

        internal IdentityTerm(string identity) =>
            this.Identity = identity;

        public override bool Reducible =>
            false;

        public override Term VisitReplace(string identity, Term replacement) =>
            (identity == this.Identity) ?
                replacement :
                this;

        public override Term VisitReduce() =>
            this;

        public override int GetHashCode() =>
            this.Identity.GetHashCode();

        public bool Equals(IdentityTerm? other) =>
            other?.Identity.Equals(this.Identity) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as IdentityTerm);

        public override string ToString() =>
            this.Identity;
    }
}
