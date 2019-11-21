using Favalon.ParseRunners;
using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;

namespace Favalon
{
    partial class Environment
    {
        public IEnumerable<Term> Parse(
            IEnumerable<Token> tokens)
        {
            var runnerContext = ParseRunnerContext.Create(this);
            var runner = WaitingRunner.Instance;

            foreach (var token in tokens)
            {
                switch (runner.Run(runnerContext, token))
                {
                    case ParseRunnerResult(ParseRunner next, Term term):
                        yield return term;
                        runner = next;
                        break;
                    case ParseRunnerResult(ParseRunner next, _):
                        runner = next;
                        break;
                }

                runnerContext.LastToken = token;
            }

            // Exhaust all saved scopes
            while (ParserUtilities.LeaveScope(runnerContext, null));

            // Contains final result
            if (runnerContext.CurrentTerm is Term finalTerm)
            {
                yield return finalTerm;
            }
        }
    }
}
