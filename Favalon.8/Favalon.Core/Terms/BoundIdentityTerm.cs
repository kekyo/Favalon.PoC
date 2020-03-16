namespace Favalon.Terms
{
    public sealed class BoundIdentityTerm :
        VariableTerm
    {
        internal BoundIdentityTerm(string identity) :
            base(identity)
        { }

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            this;

        protected internal override sealed Term VisitReduce(Context context) =>
            this;
    }
}
