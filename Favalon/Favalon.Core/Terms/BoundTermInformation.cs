using System.Runtime.CompilerServices;

namespace Favalon.Terms
{
    public struct BoundTermInformation
    {
        public readonly bool Infix;
        public readonly bool RightToLeft;
        public readonly BoundTermPrecedences Precedence;
        public readonly Term Term;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal BoundTermInformation(bool infix, bool rightToLeft, BoundTermPrecedences precedence, Term term)
        {
            this.Infix = infix;
            this.RightToLeft = rightToLeft;
            this.Precedence = precedence;
            this.Term = term;
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Deconstruct(out bool infix, out bool rightToLeft, out BoundTermPrecedences precedence, out Term term)
        {
            infix = this.Infix;
            rightToLeft = this.RightToLeft;
            precedence = this.Precedence;
            term = this.Term;
        }
    }
}
