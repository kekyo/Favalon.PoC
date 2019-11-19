using Favalon.Terms;
using Favalon.Tokens;

namespace Favalon.ParseRunners
{
    internal struct ParenthesisScope
    {
        public readonly Term? SavedTerm;
        public readonly ParenthesisPair Pair;

        public ParenthesisScope(Term? reservedTerm, ParenthesisPair pair)
        {
            this.SavedTerm = reservedTerm;
            this.Pair = pair;
        }
    }
}
