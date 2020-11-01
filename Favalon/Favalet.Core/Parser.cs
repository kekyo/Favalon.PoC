using Favalet.Parsers;
using Favalet.Expressions;
using Favalet.Tokens;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet
{
    public interface IParser
    {
        IEnumerable<IExpression> Parse(IEnumerable<Token> tokens);
    }
    
    public sealed class Parser : IParser
    {
#if DEBUG
        public int BreakIndex = -1;
#endif

        [DebuggerStepThrough]
        private Parser()
        {
        }

        public IEnumerable<IExpression> Parse(IEnumerable<Token> tokens)
        {
            var runnerContext = ParseRunnerContext.Create(this);
            var runner = WaitingRunner.Instance;
#if DEBUG
            var index = 0;
#endif
            foreach (var token in tokens)
            {
#if DEBUG
                if (index == BreakIndex) Debugger.Break();
                index++;
#endif
                switch (runner.Run(runnerContext, token))
                {
                    case ParseRunnerResult(ParseRunner next, IExpression term):
                        yield return term;
                        runner = next;
                        break;
                    case ParseRunnerResult(ParseRunner next, _):
                        runner = next;
                        break;
                }

                Debug.WriteLine($"{index - 1}: '{token}': {runnerContext}");

                runnerContext.SetLastToken(token);
            }

            // Exhaust all saved scopes
            while (ParserUtilities.LeaveOneImplicitScope(runnerContext) != LeaveScopeResults.None);

            // Contains final result
            if (runnerContext.CurrentTerm is IExpression finalTerm)
            {
                // Iterate with unveiling hided terms.
                yield return finalTerm.VisitUnveil();
            }
        }

        public static Parser Create() =>
            new Parser();
    }
}
