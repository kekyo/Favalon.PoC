namespace Favalon.Terms
{
    public sealed class BoundIdentityTerm :
        VariableTerm
    {
        public readonly BoundAssociatives Associative;

        internal BoundIdentityTerm(string identity, BoundAssociatives associative) :
            base(identity) =>
            this.Associative = associative;

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            this;

        protected override string VisitTermString(bool includeTermName) =>
            $"{this.Name}@{this.Associative}";

        public void Deconstruct(out string name, out BoundAssociatives associative)
        {
            name = this.Name;
            associative = this.Associative;
        }
    }
}
