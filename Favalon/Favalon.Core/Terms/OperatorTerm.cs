namespace Favalon.Terms
{
    public sealed class OperatorTerm :
        VariableTerm
    {
        internal OperatorTerm(string identity) :
            base(identity)
        { }

        protected internal override Term VisitReplace(string identity, Term replacement) =>
            this;
    }
}
