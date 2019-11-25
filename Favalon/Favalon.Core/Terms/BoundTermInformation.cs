using System.Runtime.CompilerServices;

namespace Favalon.Terms
{
    public struct BoundTermInformation
    {
        public readonly BoundTermNotations Notation;
        public readonly bool RightToLeft;
        public readonly BoundTermPrecedences Precedence;
        public readonly Term Term;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal BoundTermInformation(
            BoundTermNotations notation,
            bool rightToLeft,
            BoundTermPrecedences precedence,
            Term term)
        {
            this.Notation = notation;
            this.RightToLeft = rightToLeft;
            this.Precedence = precedence;
            this.Term = term;
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Deconstruct(
            out BoundTermNotations notation,
            out bool rightToLeft,
            out BoundTermPrecedences precedence,
            out Term term)
        {
            notation = this.Notation;
            rightToLeft = this.RightToLeft;
            precedence = this.Precedence;
            term = this.Term;
        }
    }
}
