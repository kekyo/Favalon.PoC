using Favalon.ParseRunners;
using Favalon.Terms;
using Favalon.Tokens;
using System;
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

            // Exhaust saved scopes
            while (runnerContext.Scopes.Count >= 1)
            {
                var parenthesisScope = runnerContext.Scopes.Pop();
                if (parenthesisScope.ParenthesisPair is ParenthesisPair parenthesisPair)
                {
                    throw new InvalidOperationException(
                        $"Unmatched parenthesis: {parenthesisPair}");
                }
                runnerContext.CurrentTerm = ParseRunner.CombineTerms(
                    parenthesisScope.SavedTerm,
                    runnerContext.CurrentTerm);
            }

            if (runnerContext.CurrentTerm is Term finalTerm)
            {
                yield return finalTerm;
            }
        }
    }
}
