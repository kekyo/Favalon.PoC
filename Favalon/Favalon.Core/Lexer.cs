﻿using Favalon.LexRunners;
using Favalon.Tokens;
using System.Collections.Generic;
using System.IO;

namespace Favalon
{
    public static class Lexer
    {
        public static IEnumerable<Token> EnumerableTokens(string text)
        {
            var context = RunContext.Create();
            var runner = WaitingRunner.Instance;

            for (var index = 0; index < text.Length; index++)
            {
                switch (runner.Run(context, text[index]))
                {
                    case RunResult(Runner next, Token token0, Token token1):
                        yield return token0;
                        yield return token1;
                        runner = next;
                        break;
                    case RunResult(Runner next, Token token, _):
                        yield return token;
                        runner = next;
                        break;
                    case RunResult(Runner next, _, _):
                        runner = next;
                        break;
                }
            }

            if (runner.Finish(context) is RunResult(_, Token finalToken, _))
            {
                yield return finalToken;
            }
        }

        public static IEnumerable<Token> EnumerableTokens(IEnumerable<char> chars)
        {
            var context = RunContext.Create();
            var runner = WaitingRunner.Instance;

            foreach (var ch in chars)
            {
                switch (runner.Run(context, ch))
                {
                    case RunResult(Runner next, Token token0, Token token1):
                        yield return token0;
                        yield return token1;
                        runner = next;
                        break;
                    case RunResult(Runner next, Token token, _):
                        yield return token;
                        runner = next;
                        break;
                    case RunResult(Runner next, _, _):
                        runner = next;
                        break;
                }
            }

            if (runner.Finish(context) is RunResult(_, Token finalToken, _))
            {
                yield return finalToken;
            }
        }

        public static IEnumerable<Token> EnumerableTokens(TextReader tr)
        {
            var context = RunContext.Create();
            var runner = WaitingRunner.Instance;

            while (true)
            {
                var inch = tr.Read();
                if (inch < 0)
                {
                    break;
                }

                switch (runner.Run(context, (char)inch))
                {
                    case RunResult(Runner next, Token token0, Token token1):
                        yield return token0;
                        yield return token1;
                        runner = next;
                        break;
                    case RunResult(Runner next, Token token, _):
                        yield return token;
                        runner = next;
                        break;
                    case RunResult(Runner next, _, _):
                        runner = next;
                        break;
                }
            }

            if (runner.Finish(context) is RunResult(_, Token finalToken, _))
            {
                yield return finalToken;
            }
        }
    }
}
