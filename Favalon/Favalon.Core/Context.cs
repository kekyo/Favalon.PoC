using Favalon.Terms;
using System.Collections.Generic;

namespace Favalon
{
    public class Context
    {
        private protected Context()
        { }

        public Term Replace(Term term, string identity, Term replacement) =>
            term.VisitReplace(identity, replacement);

        public IEnumerable<Term> EnumerableReduceSteps(Term term)
        {
            var current = term;
            while (true)
            {
                yield return current;

                var reduced = current.VisitReduce(this);
                if (object.ReferenceEquals(reduced, current))
                {
                    break;
                }

                current = reduced;
            }
        }

        public Term Reduce(Term term, bool willFinish = true)
        {
            var current = term;
            do
            {
                var reduced = current.VisitReduce(this);
                if (object.ReferenceEquals(reduced, current))
                {
                    break;
                }
                current = reduced;
            }
            while (willFinish);

            return current;
        }

        public Term Call(CallableTerm term, Term argument) =>
            term.VisitCall(this, argument);
    }
}
