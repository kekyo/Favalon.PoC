using Favalon.Terms;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Favalon
{
    public enum BoundTermNotations
    {
        Prefix,
        Infix
    }

    public enum BoundTermAssociatives
    {
        LeftToRight,
        RightToLeft
    }

    public enum BoundTermPrecedences
    {
        Lowest = 0,
        Morphism = 1000,
        ArithmericAddition = 2000,
        ArithmericMultiplication = 3000,
        Apply = 5000,
    }

    public abstract class Context
    {
        private readonly Dictionary<string, List<BoundTermInformation>> boundTerms;

        private protected Context() =>
            boundTerms = new Dictionary<string, List<BoundTermInformation>>();

        private protected Context(Dictionary<string, List<BoundTermInformation>> initialBoundTerm) =>
            boundTerms = new Dictionary<string, List<BoundTermInformation>>(initialBoundTerm);

        private protected static void AddBoundTerm(
            Dictionary<string, List<BoundTermInformation>> boundTerms,
            string identity,
            BoundTermNotations notation,
            bool rightToLeft,
            BoundTermPrecedences precedence,
            Term term)
        {
            if (!boundTerms.TryGetValue(identity, out var terms))
            {
                terms = new List<BoundTermInformation>();
                boundTerms.Add(identity, terms);
            }
            terms.Add(new BoundTermInformation(notation, rightToLeft, precedence, term));
        }

        public void AddBoundTerm(
            string identity,
            BoundTermNotations notation,
            bool rightToLeft,
            int precedence,
            Term term) =>
            AddBoundTerm(boundTerms, identity, notation, rightToLeft, (BoundTermPrecedences)precedence, term);

        public void AddBoundTerm(
            string identity,
            BoundTermNotations notation,
            bool rightToLeft,
            BoundTermPrecedences precedence,
            Term term) =>
            AddBoundTerm(boundTerms, identity, notation, rightToLeft, precedence, term);

        public BoundTermInformation[]? LookupBoundTerms(string identity) =>
            boundTerms.TryGetValue(identity, out var terms) ? terms.ToArray() : null;

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
