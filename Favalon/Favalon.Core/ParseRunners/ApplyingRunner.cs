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
            Debug.Assert(context.PreSignToken == null);

            // Ignore WillApplyRightToLeft checking if token is whitespace.
            if (token is WhiteSpaceToken)
            {
                return ParseRunnerResult.Empty(this);
            }

            // Triggered the token arranging by RTL.
            if (context.WillApplyRightToLeft)
            {
                if (token is ValueToken)
                {
                    context.PushScope();
                    context.CurrentTerm = null;
                }

                context.WillApplyRightToLeft = false;
            }

            switch (token)
            {
                case IdentityToken identity:
                    return ParserUtilities.RunIdentity(context, identity);

                case NumericToken numeric:
                    context.CurrentTerm = ParserUtilities.CombineTerms(
                        context.CurrentTerm,
                        ParserUtilities.GetNumericConstant(numeric.Value, NumericalSignes.Plus));
                    return ParseRunnerResult.Empty(this);

                case NumericalSignToken numericSign:
                    // "abc -" / "123 -" ==> binary op or signed
                    if (context.LastToken is WhiteSpaceToken)
                    {
                        context.PreSignToken = numericSign;
                        return ParseRunnerResult.Empty(NumericalSignedRunner.Instance);
                    }
                    // "abc-" / "123-" / "(abc)-" ==> binary op
                    else
                    {
                        context.CurrentTerm = ParserUtilities.CombineTerms(
                            context.CurrentTerm,
                            new IdentityTerm(numericSign.Symbol.ToString()));
                        return ParseRunnerResult.Empty(this);
                    }

                case OpenParenthesisToken parenthesis:
                    context.PushScope(parenthesis.Pair);
                    return ParseRunnerResult.Empty(WaitingRunner.Instance);

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

        public static readonly ParseRunner Instance = new ApplyingRunner();
    }
}
