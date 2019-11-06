﻿using System;

namespace Favalon.Terms
{
    public sealed class IdentityTerm :
        Term, IEquatable<IdentityTerm?>
    {
        public new readonly string Identity;

        internal IdentityTerm(string identity) =>
            this.Identity = identity;

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            (identity == this.Identity) ?
                replacement :
                this;

        protected internal override Term VisitReduce(Context context) =>
            context.LookupIdentity(this) is Term[] terms ?
                terms[0] :
                this;

        public override int GetHashCode() =>
            this.Identity.GetHashCode();

        public bool Equals(IdentityTerm? other) =>
            other?.Identity.Equals(this.Identity) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as IdentityTerm);

        protected override string VisitTermString(bool includeTermName) =>
            this.Identity;

        public void Deconstruct(out string identity) =>
            identity = this.Identity;
    }
}
