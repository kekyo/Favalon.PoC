namespace Favalon.Terms
{
    public sealed class BoundIdentityTerm :
        VariableTerm
    {
        internal BoundIdentityTerm(string identity, Term? higherOrder) :
            base(identity) =>
            this.HigherOrder = higherOrder;

        public override Term? HigherOrder { get; }

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            this;

        protected internal override sealed Term VisitReduce(Context context) =>
            this;
    }
}
