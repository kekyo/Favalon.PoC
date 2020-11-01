using Favalet.Expressions;
using Favalet.Tokens;
using System;
using System.Diagnostics;

namespace Favalet.Parsers
{
    internal sealed class ApplyingRunner : ParseRunner
    {
        private ApplyingRunner()
        { }

        public override ParseRunnerResult Run(ParseRunnerContext runnerContext, Token token)
        {
            Debug.Assert(runnerContext.CurrentTerm != null);
            Debug.Assert(runnerContext.PreSignToken == null);

            // Ignore WillApplyRightToLeft checking if token is whitespace.
            if (token is WhiteSpaceToken)
            {
                return ParseRunnerResult.Empty(this);
            }

            // Triggered the token arranging by RTL.
            if (runnerContext.ApplyNextAssociative == BoundTermAssociatives.RightToLeft)
            {
                if (token is ValueToken)
                {
                    runnerContext.PushScope();
                }

                runnerContext.ApplyNextAssociative = BoundTermAssociatives.LeftToRight;
            }

            switch (token)
            {
                case IdentityToken identity:
                    return ParserUtilities.RunIdentity(runnerContext, identity);

                case NumericToken numeric:
                    runnerContext.CombineAfter(
                        ParserUtilities.GetNumericConstant(numeric.Value, NumericalSignes.Plus));
                    return ParseRunnerResult.Empty(this);

                case NumericalSignToken numericSign:
                    // "abc -" / "123 -" ==> binary op or signed
                    if (runnerContext.LastToken is WhiteSpaceToken)
                    {
                        runnerContext.PreSignToken = numericSign;
                        return ParseRunnerResult.Empty(NumericalSignedRunner.Instance);
                    }
                    // "abc-" / "123-" / "(abc)-" ==> binary op
                    else
                    {
                        runnerContext.CombineAfter(
                            new IdentityTerm(numericSign.Symbol.ToString()));
                        return ParseRunnerResult.Empty(this);
                    }

                case OpenParenthesisToken parenthesis:
                    runnerContext.PushScope(parenthesis.Pair);
                    return ParseRunnerResult.Empty(WaitingRunner.Instance);

                case CloseParenthesisToken parenthesis:
                    while (true)
                    {
                        var result = ParserUtilities.LeaveOneScope(runnerContext, parenthesis.Pair);
                        Debug.Assert(result != LeaveScopeResults.None);
                        if (result == LeaveScopeResults.Explicitly)
                        {
                            break;
                        }
                    }
                    return ParseRunnerResult.Empty(this);

                default:
                    throw new InvalidOperationException(token.ToString());
            }
        }

        public static readonly ParseRunner Instance = new ApplyingRunner();
    }
}
