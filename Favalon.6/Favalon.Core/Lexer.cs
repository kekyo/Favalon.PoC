using System.Collections.Generic;

using Favalon.Internals;
using Favalon.Tokens;

namespace Favalon
{
    public sealed class Lexer
    {
        public static string OperatorChars => LexerCore.operatorChars;

        private readonly LexerCore lexerCore = new LexerCore();

        public Token? Tokenize(char inch) =>
            lexerCore.Examine(inch);

        public IEnumerable<Token> Tokenize(string input, bool withFlush = true)
        {
            foreach (var inch in input)
            {
                if (lexerCore.Examine(inch) is Token token)
                {
                    yield return token;
                }
            }

            if (withFlush)
            {
                if (lexerCore.Flush() is Token token)
                {
                    yield return token;
                }
            }
        }

        public Token? Flush() =>
            lexerCore.Flush();
    }
}
