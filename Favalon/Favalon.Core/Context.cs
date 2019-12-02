using Favalon.Internal;
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
        Method = 100,
        Function = 1000,
        Morphism = 3000,
        ArithmericAddition = 5000,
        ArithmericMultiplication = 6000,
    }

    public struct BoundTermInformation
    {
        public readonly BoundTermNotations Notation;
        public readonly BoundTermAssociatives Associative;
        public readonly BoundTermPrecedences? Precedence;
        public readonly Term Term;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal BoundTermInformation(
            BoundTermNotations notation,
            BoundTermAssociatives associative,
            BoundTermPrecedences? precedence,
            Term term)
        {
            this.Notation = notation;
            this.Associative = associative;
            this.Precedence = precedence;
            this.Term = term;
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Deconstruct(
            out BoundTermNotations notation,
            out BoundTermAssociatives associative,
            out BoundTermPrecedences? precedence,
            out Term term)
        {
            notation = this.Notation;
            associative = this.Associative;
            precedence = this.Precedence;
            term = this.Term;
        }
    }

    public class Context
    {
        private protected readonly ManagedDictionary<string, List<BoundTermInformation>> boundTerms;

        private protected Context(ManagedDictionary<string, List<BoundTermInformation>> initialBoundTerm) =>
            boundTerms = initialBoundTerm;

        internal Context Clone() =>
            new Context(boundTerms.Clone());

        internal static void AddBoundTerm(
            ManagedDictionary<string, List<BoundTermInformation>> boundTerms,
            string identity,
            BoundTermNotations notation,
            BoundTermAssociatives associative,
            BoundTermPrecedences? precedence,
            Term term)
        {
            if (!boundTerms.TryGetValue(identity, out var terms))
            {
                terms = new List<BoundTermInformation>();
                boundTerms[identity] = terms;
            }
            terms.Add(new BoundTermInformation(notation, associative, precedence, term));
        }

        public void AddBoundTerm(
            string identity,
            BoundTermNotations notation,
            BoundTermAssociatives associative,
            int precedence,
            Term term) =>
            AddBoundTerm(boundTerms, identity, notation, associative, (BoundTermPrecedences)precedence, term);

        public void AddBoundTerm(
            string identity,
            BoundTermNotations notation,
            BoundTermAssociatives associative,
            BoundTermPrecedences? precedence,
            Term term) =>
            AddBoundTerm(boundTerms, identity, notation, associative, precedence, term);

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

        public Term Infer(Term term) =>
            term.VisitInfer(this);
    }
}
