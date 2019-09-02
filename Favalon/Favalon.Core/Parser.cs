using System.Collections.Generic;

using Favalon.Parsing;

namespace Favalon
{
    public sealed class Parser
    {
        public static string OperatorChars => ParserCore.operatorChars;

        private readonly ParserCore parser = new ParserCore();

        public Token? Tokenize(char inch) =>
            parser.Run(inch);

        public IEnumerable<Token> Tokenize(string input, bool withFlush = true)
        {
            var index = 0;
            while (index < input.Length)
            {
                var inch = input[index++];
                if (parser.Run(inch) is Token token)
                {
                    yield return token;
                }
            }

            if (withFlush)
            {
                if (parser.Flush() is Token token)
                {
                    yield return token;
                }
            }
        }

        public Token? Flush() =>
            parser.Flush();
    }
}
