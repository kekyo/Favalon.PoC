namespace Favalon.Terms
{
    public abstract partial class Term
    {
        protected internal abstract Term VisitReplace(string identity, Term replacement);

        protected internal abstract Term VisitReduce(Context context);
    }
}
