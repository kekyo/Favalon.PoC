using Favalon.Terms;
using Favalon.Tokens;
using System.Runtime.CompilerServices;

namespace Favalon.ParseRunners
{
    internal struct ScopeInformation
    {
        public readonly Term? SavedTerm;
        public readonly ParenthesisPair? ParenthesisPair;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ScopeInformation(Term? reservedTerm, ParenthesisPair? parenthesisPair = null)
        {
            this.SavedTerm = reservedTerm;
            this.ParenthesisPair = parenthesisPair;
        }
    }
}
