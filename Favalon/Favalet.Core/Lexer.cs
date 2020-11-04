using Favalet.Lexers;
using Favalet.Tokens;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Favalet
{
    public interface ILexer
    {
        IEnumerable<Token> EnumerableTokens(IEnumerable<char> chars);
    }
    
    public sealed class Lexer : ILexer
    {
        [DebuggerStepThrough]
        private Lexer()
        {
        }

        public IEnumerable<Token> EnumerableTokens(IEnumerable<char> chars)
        {
            var runnerContext = LexRunnerContext.Create();
            var runner = WaitingIgnoreSpaceRunner.Instance;

            foreach (var ch in chars)
            {
                switch (runner.Run(runnerContext, ch))
                {
                    case LexRunnerResult(LexRunner next, Token token0, Token token1):
                        yield return token0;
                        yield return token1;
                        runner = next;
                        break;
                    case LexRunnerResult(LexRunner next, Token token, _):
                        yield return token;
                        runner = next;
                        break;
                    case LexRunnerResult(LexRunner next, _, _):
                        runner = next;
                        break;
                }
            }

            if (runner.Finish(runnerContext) is LexRunnerResult(_, Token finalToken, _))
            {
                yield return finalToken;
            }
        }
        
        [DebuggerStepThrough]
        public static Lexer Create() =>
            new Lexer();
    }

    [DebuggerStepThrough]
    public static class LexerExtension
    {
        public static IEnumerable<Token> EnumerableTokens(this ILexer lexer, string text)
        {
            IEnumerable<char> Iterator()
            {
                foreach (var inch in text)
                {
                    yield return inch;
                }
            }
            
            return lexer.EnumerableTokens(Iterator());
        }

        public static IEnumerable<Token> EnumerableTokens(this ILexer lexer, TextReader tr)
        {
            TextReader? current = tr;
            IEnumerable<char> Iterator()
            {
                if (current == null)
                {
                    yield break;
                }
                
                while (true)
                {
                    var inch = current.Read();
                    if (inch < 0)
                    {
                        break;
                    }
                    yield return (char)inch;
                }

                current = null;
            }
            
            return lexer.EnumerableTokens(Iterator());
        }
    }
}
