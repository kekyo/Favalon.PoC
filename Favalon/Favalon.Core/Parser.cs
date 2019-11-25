using Favalon.ParseRunners;
using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalon
{
    partial class Environment
    {
#if DEBUG
        public int BreakIndex = -1;
#endif

        public IEnumerable<Term> Parse(IEnumerable<Token> tokens)
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
                    case ParseRunnerResult(ParseRunner next, Term term):
                        yield return term;
                        runner = next;
                        break;
                    case ParseRunnerResult(ParseRunner next, _):
                        runner = next;
                        break;
                }

                Debug.WriteLine($"{index - 1}: '{token}': {runnerContext}");

                runnerContext.LastToken = token;
            }

            // Exhaust all saved scopes
            while (ParserUtilities.LeaveOneImplicitScope(runnerContext) != LeaveScopeResults.None);

            // Contains final result
            if (runnerContext.CurrentTerm is Term finalTerm)
            {
                // Iterate with unveiling hided terms.
                yield return finalTerm.VisitUnveil();
            }
        }
    }
}
