using Favalon.Terms;

namespace Favalon.ParseRunners
{
    internal struct OneOfTermInformation
    {
        public readonly Term? Term;
        public readonly char Close;

        public OneOfTermInformation(Term? term, char close)
        {
            this.Term = term;
            this.Close = close;
        }
    }
}
