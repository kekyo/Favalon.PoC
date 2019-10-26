using System;

namespace Favalon.Terms
{
    public sealed class IdentityTerm : Term
    {
        public readonly string Name;

        internal IdentityTerm(string name) =>
            this.Name = name;

        public override bool Reducible =>
            false;

        public override Term VisitReplace(string name, Term term) =>
            (name == this.Name) ?
                term :
                this;

        public override Term VisitReduce() =>
            this;

        public override string ToString() =>
            this.Name;
    }
}
