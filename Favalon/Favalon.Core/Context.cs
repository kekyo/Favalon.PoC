using Favalon.Internal;
using Favalon.Terms;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    public class Context
    {
        private static readonly Dictionary<string, List<Term>> boundTerms =
            typeof(object).GetTypeInfo().Assembly.
            EnumerableAllPublicStaticMethods().
            Where(method => method.GetParameters().Length == 1).
            GroupBy(method => method.GetFullName()).
            ToDictionary(g => g.Key, g => g.Select(method => (Term)new MethodTerm(method)).ToList());

        private static void AddBoundTerm(
            Dictionary<string, List<Term>> boundTerms, string identity, Term term)
        {
            if (!boundTerms.TryGetValue(identity, out var terms))
            {
                terms = new List<Term>();
                boundTerms.Add(identity, terms);
            }
            terms.Add(term);
        }

        static Context()
        {
            // operator arrow (lambda constructor)
            // -> a b
            // --------------
            // f  a b
            // ((f:'1->'2 a:'1):'2 b:'3):'4
            // ((f:'1->'2 a:'1):'3->'4 b:'3):'4
            // ((f:'1->'3->'4 a:'1):'3->'4 b:'3):'4
            AddBoundTerm(
                boundTerms,
                "->",
                // f:'1->'3->'4
                new InterpretTerm(
                    "->", "a",  // a:'1
                    (ic, a) =>
                        // '3->'4
                        new InterpretTerm(
                            $"Closure(-> {a})", "b",  // b:'3
                            (oc, b) =>
                                new FunctionTerm((IdentityTerm)a.VisitReduce(ic), b.VisitReduce(oc)))));
        }

        private protected Context()
        { }

        public Term[]? LookupBoundTerms(VariableTerm variable) =>
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
