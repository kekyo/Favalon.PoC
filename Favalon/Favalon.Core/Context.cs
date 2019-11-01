using Favalon.Terms;

namespace Favalon
{
    public class Context
    {
        private protected Context()
        { }

        public Term Reduce(Term term)
        {
            var current = term;
            while (true)
            {
                var reduced = current.VisitReduce(this);
                if (object.ReferenceEquals(reduced, current))
                {
                    return current;
                }
                current = reduced;
            }
        }
    }
}
