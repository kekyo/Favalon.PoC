namespace Favalon.Terms
{
    public abstract partial class Term
    {
        public abstract Term VisitReplace(string identity, Term replacement);

        public abstract Term VisitReduce(Context context);
    }
}
