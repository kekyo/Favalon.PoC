using Favalon.Internals;
using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;

namespace Favalon
{
    public sealed class Parser
    {
        private readonly ParserCore parserCore = new ParserCore();

        public IEnumerable<Term> Parse(IEnumerable<Token> tokens, bool withFlush = true)
        {
            foreach (var token in tokens)
            {
                if (parserCore.Examine(token) is Term term)
                {
                    yield return term;
                }
            }

            if (withFlush)
            {
                if (parserCore.Flush() is Term term)
                {
                    yield return term;
                }
            }
        }

        public Term? Flush() =>
            parserCore.Flush();
    }
}
