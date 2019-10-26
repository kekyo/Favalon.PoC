using Favalon.Tokens;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Favalon
{
    public sealed class Lexer
    {
        private enum States
        {
            Waiting,
            Identity
        }

        private States state = States.Waiting;
        private readonly StringBuilder tokenBuffer = new StringBuilder();

        private Lexer()
        { }

        private Token? Run(char ch)
        {
            switch (state)
            {
                case States.Waiting when (!char.IsWhiteSpace(ch) && !char.IsControl(ch)):
                    tokenBuffer.Append(ch);
                    state = States.Identity;
                    return null;
                case States.Identity:
                    if (char.IsWhiteSpace(ch))
                    {
                        var token = tokenBuffer.ToString();
                        tokenBuffer.Clear();
                        state = States.Waiting;
                        return new IdentityToken(token);
                    }
                    else
                    {
                        tokenBuffer.Append(ch);
                        return null;
                    }
                default:
                    return null;
            }
        }

        private Token? Finish()
        {
            switch (state)
            {
                case States.Identity:
                    var token = tokenBuffer.ToString();
                    tokenBuffer.Clear();
                    state = States.Waiting;
                    return new IdentityToken(token);
                default:
                    return null;
            }
        }

        public static IEnumerable<Token> EnumerableTokens(TextReader tr)
        {
            var lexer = new Lexer();

            while (true)
            {
                var inch = tr.Read();
                if (inch < 0)
                {
                    break;
                }

                if (lexer.Run((char)inch) is Token token)
                {
                    yield return token;
                }
            }

            if (lexer.Finish() is Token finalToken)
            {
                yield return finalToken;
            }
        }
    }
}
