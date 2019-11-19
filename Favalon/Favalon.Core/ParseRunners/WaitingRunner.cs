using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Diagnostics;

namespace Favalon.ParseRunners
{
    internal sealed class WaitingRunner : ParseRunner
    {
        private WaitingRunner()
        { }

        public override ParseRunnerResult Run(ParseRunnerContext context, Token token)
        {
            Debug.Assert(context.CurrentTerm == null);

            switch (token)
            {
                case IdentityToken identity:
                    context.CurrentTerm = CombineTerm(
                        context.CurrentTerm,
                        new IdentityTerm(identity.Identity));
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                case OpenParenthesisToken parenthesis:
                    context.ParenthesisScopes.Push(
                        new ParenthesisScope(context.CurrentTerm, parenthesis.Pair));
                    context.CurrentTerm = null;
                    return ParseRunnerResult.Empty(
                        this);

                default:
                    throw new InvalidOperationException();
            }
        }

        public static readonly ParseRunner Instance = new WaitingRunner();
    }
}
