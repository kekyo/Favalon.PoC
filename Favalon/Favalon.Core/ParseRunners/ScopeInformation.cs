using Favalon.Terms;
using Favalon.Tokens;
using System.Runtime.CompilerServices;

namespace Favalon.ParseRunners
{
    internal struct ScopeInformation
    {
        public readonly Term? SavedTerm;
        public readonly BoundTermPrecedences? Precedence;
        public readonly ParenthesisPair? ParenthesisPair;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ScopeInformation(
            Term? reservedTerm,
            BoundTermPrecedences? precedence,
            ParenthesisPair? parenthesisPair)
        {
            this.SavedTerm = reservedTerm;
            this.Precedence = precedence;
            this.ParenthesisPair = parenthesisPair;
        }
    }
}
