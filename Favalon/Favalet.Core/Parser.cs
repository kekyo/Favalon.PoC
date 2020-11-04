using Favalet.Expressions;
using Favalet.Tokens;
using Favalet.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet
{
    public interface IParser
    {
        IEnumerable<IExpression> Parse(IEnumerable<Token> tokens);
    }
    
    public class Parser : IParser
    {
#if DEBUG
        public int BreakIndex = -1;
#endif
        private readonly ParseRunnerFactory factory;

        [DebuggerStepThrough]
        protected Parser(ParseRunnerFactory factory) =>
            this.factory = factory;

        public IEnumerable<IExpression> Parse(IEnumerable<Token> tokens)
        {
#if DEBUG
            var index = 0;
#endif
            var context = this.factory.CreateContext();
            var runner = factory.Waiting;
            
            foreach (var token in tokens)
            {
#if DEBUG
                if (index == BreakIndex) Debugger.Break();
                index++;
#endif
                switch (runner.Run(context, factory, token))
                {
                    case ParseRunnerResult(ParseRunner next, IExpression expression):
                        yield return expression;
                        runner = next;
                        break;
                    case ParseRunnerResult(ParseRunner next, _):
                        runner = next;
                        break;
                }

                Debug.WriteLine($"{index - 1}: '{token}': {context}");

                context.SetLastToken(token);
            }

            // Contains final result
            if (context.Current is IExpression finalTerm)
            {
                yield return finalTerm;
            }
        }

        public static readonly Parser Instance =
            new Parser(ParseRunnerFactory.Instance);
    }
}
