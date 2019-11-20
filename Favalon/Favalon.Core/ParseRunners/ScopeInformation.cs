using Favalon.Terms;
using Favalon.Tokens;

namespace Favalon.ParseRunners
{
    internal struct ScopeInformation
    {
        public readonly Term? SavedTerm;
        public readonly ParenthesisPair? ParenthesisPair;

        public ScopeInformation(Term? reservedTerm, ParenthesisPair? parenthesisPair = null)
        {
            this.SavedTerm = reservedTerm;
            this.ParenthesisPair = parenthesisPair;
        }
    }
}
