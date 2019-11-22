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
            Debug.Assert(context.WillApplyRightToLeft == false);

            if (token is WhiteSpaceToken)
            {
                return ParseRunnerResult.Empty(this);
            }

            switch (token)
            {
                // "abc"
                case IdentityToken identity:
                    return ParserUtilities.RunIdentity(context, identity);

                // "123"
                case NumericToken numeric:
                    // Initial precedence (Apply)
                    context.CurrentPrecedence = BoundTermPrecedences.Apply;

                    context.CurrentTerm = ParserUtilities.CombineTerms(
                        context.CurrentTerm,
                        ParserUtilities.GetNumericConstant(numeric.Value, NumericalSignes.Plus));
                    return ParseRunnerResult.Empty(ApplyingRunner.Instance);

                // "-"
                case NumericalSignToken numericSign:
                    context.PreSignToken = numericSign;
                    return ParseRunnerResult.Empty(NumericalSignedRunner.Instance);

                // "("
                case OpenParenthesisToken parenthesis:
                    context.PushScope(parenthesis.Pair);
                    return ParseRunnerResult.Empty(this);

                // ")"
                case CloseParenthesisToken parenthesis:
                    if (ParserUtilities.LeaveScopes(context, parenthesis.Pair))
                    {
                        return ParseRunnerResult.Empty(this);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Couldn't find open parenthesis: '{parenthesis.Pair.Open}'");
                    }

                default:
                    throw new InvalidOperationException(token.ToString());
            }
        }

        public static readonly ParseRunner Instance = new WaitingRunner();
    }
}
