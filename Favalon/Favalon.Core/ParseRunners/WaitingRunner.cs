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
            Debug.Assert(context.PreSignToken == null);

            switch (token)
            {
                // "a"
                case IdentityToken identity:
                    context.CurrentTerm = CombineTerms(
                        context.CurrentTerm,
                        new IdentityTerm(identity.Identity));
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                // "("
                case OpenParenthesisToken parenthesis:
                    context.Scopes.Push(
                        new ScopeInformation(context.CurrentTerm, parenthesis.Pair));
                    context.CurrentTerm = null;
                    return ParseRunnerResult.Empty(
                        this);

                // "1"
                case NumericToken numeric:
                    context.CurrentTerm = CombineTerms(
                        context.CurrentTerm,
                        GetNumericConstant(numeric.Value, Signes.Plus));
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                // "-"
                case NumericalSignToken numericSign:
                    context.PreSignToken = numericSign;
                    return ParseRunnerResult.Empty(
                        NumericalSignedRunner.Instance);

                default:
                    throw new InvalidOperationException();
            }
        }

        public static readonly ParseRunner Instance = new WaitingRunner();
    }
}
