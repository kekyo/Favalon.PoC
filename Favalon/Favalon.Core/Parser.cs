using Favalon.Internals;
using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;

namespace Favalon
{
    public sealed class Parser
    {
        private readonly ParserCore parserCore = new ParserCore();

        public void AddVariable(string variable, Term term) =>
            parserCore.AddVariable(variable, term);

        public IEnumerable<Term> Parse(IEnumerable<Token> tokens, bool withFlush = true)
        {
            foreach (var token in tokens)
            {
                foreach (var term in parserCore.Examine(token))
                {
                    yield return term;
                }
            }

            if (withFlush)
            {
                foreach (var term in parserCore.Flush())
                {
                    yield return term;
                }
            }
        }

        public IEnumerable<Term> Flush() =>
            parserCore.Flush();
    }
}
