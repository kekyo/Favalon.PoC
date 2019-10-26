namespace Favalon.Terms
{
    public abstract partial class Term
    {
        public abstract bool Reducible { get; }

        public abstract Term VisitReplace(string name, Term replacement);

        public abstract Term VisitReduce();

        public Term Reduce()
        {
            var current = this;
            while (current.Reducible)
            {
                current = current.VisitReduce();
            }
            return current;
        }
    }
}
