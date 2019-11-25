using System.Runtime.CompilerServices;

namespace Favalon.Terms
{
    public struct BoundTermInformation
    {
        public readonly BoundTermNotations Notation;
        public readonly BoundTermAssociatives Associative;
        public readonly BoundTermPrecedences Precedence;
        public readonly Term Term;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal BoundTermInformation(
            BoundTermNotations notation,
            BoundTermAssociatives associative,
            BoundTermPrecedences precedence,
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
            out BoundTermPrecedences precedence,
            out Term term)
        {
            notation = this.Notation;
            associative = this.Associative;
            precedence = this.Precedence;
            term = this.Term;
        }
    }
}
