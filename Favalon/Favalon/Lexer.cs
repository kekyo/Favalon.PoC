using Favalon.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Favalon
{
    public sealed class Lexer
    {
        private struct RunResult
        {
            public readonly Token? Token0;
            public readonly Token? Token1;
            
            private RunResult(Token? token0, Token? token1)
            {
                this.Token0 = token0;
                this.Token1 = token1;
            }

            public static readonly RunResult Empty =
                new RunResult(null, null);
            public static RunResult Create(Token? token0) =>
                new RunResult(token0, null);
            public static RunResult Create(Token? token0, Token? token1) =>
                new RunResult(token0, token1);

            public void Deconstruct(out Token? token0, out Token? token1)
            {
                token0 = this.Token0;
                token1 = this.Token1;
            }
        }

        private enum States
        {
            Waiting,
            Identity
        }

        private States state = States.Waiting;
        private readonly StringBuilder tokenBuffer = new StringBuilder();

        private Lexer()
        { }

        private RunResult RunWaiting(char ch)
        {
            switch (ch)
            {
                case '(':
                    return RunResult.Create(
                        BeginBracketToken.Instance);
                case ')':
                    return RunResult.Create(
                        EndBracketToken.Instance);
                default:
                    if (!char.IsWhiteSpace(ch) && !char.IsControl(ch))
                    {
                        tokenBuffer.Append(ch);
                        state = States.Identity;
                    }
                    return RunResult.Empty;
            }
        }

        private RunResult RunIdentity(char ch)
        {
            switch (ch)
            {
                case '(':
                    return RunResult.Create(
                        this.Finish(), BeginBracketToken.Instance);
                case ')':
                    return RunResult.Create(
                        this.Finish(), EndBracketToken.Instance);
                default:
                    if (char.IsWhiteSpace(ch))
                    {
                        var token = tokenBuffer.ToString();
                        tokenBuffer.Clear();
                        state = States.Waiting;
                        return RunResult.Create(new IdentityToken(token));
                    }
                    else
                    {
                        tokenBuffer.Append(ch);
                        return RunResult.Empty;
                    }
            }
        }

        private RunResult Run(char ch)
        {
            switch (state)
            {
                case States.Waiting:
                    return this.RunWaiting(ch);
                case States.Identity:
                    return this.RunIdentity(ch);
                default:
                    throw new InvalidOperationException();
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

        public static IEnumerable<Token> EnumerableTokens(string text)
        {
            var lexer = new Lexer();

            for (var index = 0; index < text.Length; index++)
            {
                switch (lexer.Run(text[index]))
                {
                    case RunResult(Token token0, Token token1):
                        yield return token0;
                        yield return token1;
                        break;
                    case RunResult(Token token, null):
                        yield return token;
                        break;
                }
            }

            if (lexer.Finish() is Token finalToken)
            {
                yield return finalToken;
            }
        }

        public static IEnumerable<Token> EnumerableTokens(IEnumerable<char> chars)
        {
            var lexer = new Lexer();

            foreach (var ch in chars)
            {
                switch (lexer.Run(ch))
                {
                    case RunResult(Token token0, Token token1):
                        yield return token0;
                        yield return token1;
                        break;
                    case RunResult(Token token, null):
                        yield return token;
                        break;
                }
            }

            if (lexer.Finish() is Token finalToken)
            {
                yield return finalToken;
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

                switch (lexer.Run((char)inch))
                {
                    case RunResult(Token token0, Token token1):
                        yield return token0;
                        yield return token1;
                        break;
                    case RunResult(Token token, null):
                        yield return token;
                        break;
                }
            }

            if (lexer.Finish() is Token finalToken)
            {
                yield return finalToken;
            }
        }
    }
}
