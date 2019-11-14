using Favalon.Terms;
using System.Collections.Generic;

namespace Favalon
{
    public class Context
    {
        private readonly Dictionary<string, List<BoundTermInformation>> boundTerms;

        private protected Context(Dictionary<string, List<BoundTermInformation>> initialBoundTerm) =>
            boundTerms = new Dictionary<string, List<BoundTermInformation>>(initialBoundTerm);

        private protected static void AddBoundTerm(
            Dictionary<string, List<BoundTermInformation>> boundTerms,
            string name, bool infix, bool rightToLeft, Term term)
        {
            if (!boundTerms.TryGetValue(name, out var terms))
            {
                terms = new List<BoundTermInformation>();
                boundTerms.Add(name, terms);
            }
            terms.Add(new BoundTermInformation(infix, rightToLeft, term));
        }

        public void AddBoundTerm(string name, bool infix, bool rightToLeft, Term term) =>
            AddBoundTerm(boundTerms, name, infix, rightToLeft, term);

        public BoundTermInformation[]? LookupBoundTerms(VariableTerm variable) =>
            boundTerms.TryGetValue(variable.Name, out var terms) ? terms.ToArray() : null;

        public Term Transpose(Term term) =>
            term.VisitTranspose(this);

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
