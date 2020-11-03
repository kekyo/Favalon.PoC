using Favalet.Expressions;
using Favalet.Tokens;
using Favalet.Parsers;
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
        [DebuggerStepThrough]
        private sealed class Factory : IParseRunnerFactory
        {
            private Factory()
            { }

            public ParseRunner Waiting { get; } = WaitingRunner.Instance;
            public ParseRunner Applying { get; } = ApplyingRunner.Instance;
            
            public static IParseRunnerFactory Instance =
                new Factory();
        }
        
#if DEBUG
        public int BreakIndex = -1;
#endif

        private readonly IParseRunnerFactory factory;

        [DebuggerStepThrough]
        protected Parser(IParseRunnerFactory factory) =>
            this.factory = factory;

        public IEnumerable<IExpression> Parse(IEnumerable<Token> tokens)
        {
#if DEBUG
            var index = 0;
#endif
            var runnerContext = ParseRunnerContext.Create(this.factory);
            var runner = runnerContext.Factory.Waiting;
            
            foreach (var token in tokens)
            {
#if DEBUG
                if (index == BreakIndex) Debugger.Break();
                index++;
#endif
                switch (runner.Run(runnerContext, token))
                {
                    case ParseRunnerResult(ParseRunner next, IExpression expression):
                        yield return expression;
                        runner = next;
                        break;
                    case ParseRunnerResult(ParseRunner next, _):
                        runner = next;
                        break;
                }

                Debug.WriteLine($"{index - 1}: '{token}': {runnerContext}");

                runnerContext.SetLastToken(token);
            }

            // Contains final result
            if (runnerContext.Current is IExpression finalTerm)
            {
                yield return finalTerm;
            }
        }

        public static readonly Parser Instance =
            new Parser(Factory.Instance);
    }
}
