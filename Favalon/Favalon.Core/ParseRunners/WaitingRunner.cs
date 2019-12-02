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
            Debug.Assert(context.CurrentPrecedence == null);
            Debug.Assert(context.PreSignToken == null);
            Debug.Assert(context.ApplyNextAssociative == BoundTermAssociatives.LeftToRight);

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
                    context.CombineAfter(
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
                    while (true)
                    {
                        var result = ParserUtilities.LeaveOneScope(context, parenthesis.Pair);
                        Debug.Assert(result != LeaveScopeResults.None);
                        if (result == LeaveScopeResults.Explicitly)
                        {
                            break;
                        }
                    }
                    return ParseRunnerResult.Empty(ApplyingRunner.Instance);

                default:
                    throw new InvalidOperationException(token.ToString());
            }
        }

        public static readonly ParseRunner Instance = new WaitingRunner();
    }
}
