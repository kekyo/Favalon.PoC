using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Diagnostics;

namespace Favalon.ParseRunners
{
    internal sealed class ApplyingRunner : ParseRunner
    {
        private ApplyingRunner()
        { }

        public override ParseRunnerResult Run(ParseRunnerContext context, Token token)
        {
            Debug.Assert(context.CurrentTerm != null);

            switch (token)
            {
                case IdentityToken identity:
                    context.CurrentTerm = CombineTerm(
                        context.CurrentTerm,
                        new IdentityTerm(identity.Identity));
                    return ParseRunnerResult.Empty(this);

                case OpenParenthesisToken parenthesis:
                    context.ParenthesisScopes.Push(
                        new ParenthesisScope(context.CurrentTerm, parenthesis.Pair));
                    context.CurrentTerm = null;
                    return ParseRunnerResult.Empty(
                        WaitingRunner.Instance);

                case CloseParenthesisToken parenthesis:
                    if (context.ParenthesisScopes.Count == 0)
                    {
                        throw new InvalidOperationException(
                            $"Couldn't find open parenthesis: '{parenthesis.Pair.Open}'");
                    }
                    var parenthesisScope = context.ParenthesisScopes.Pop();
                    if (parenthesisScope.Pair.Close != parenthesis.Pair.Close)
                    {
                        throw new InvalidOperationException(
                            $"Unmatched parenthesis: {parenthesis.Pair}");
                    }
                    context.CurrentTerm = CombineTerm(
                        parenthesisScope.SavedTerm,
                        context.CurrentTerm);
                    return ParseRunnerResult.Empty(
                        this);

                default:
                    throw new InvalidOperationException();
            }
        }

        public static readonly ParseRunner Instance = new ApplyingRunner();
    }
}
