namespace Favalon.Terms
{
    public abstract partial class Term
    {
        public abstract Term VisitReplace(string identity, Term replacement);

        public abstract Term VisitReduce();

        public Term Reduce()
        {
            var current = this;
            while (true)
            {
                var reduced = current.VisitReduce();
                if (object.ReferenceEquals(reduced, current))
                {
                    return current;
                }
                current = reduced;
            }
        }
    }
}
