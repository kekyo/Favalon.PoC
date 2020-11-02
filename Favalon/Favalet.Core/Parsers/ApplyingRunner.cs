using Favalet.Expressions;
using Favalet.Tokens;
using System;
using System.Diagnostics;

namespace Favalet.Parsers
{
    public class ApplyingRunner : ParseRunner
    {
        [DebuggerStepThrough]
        protected ApplyingRunner()
        { }

        public override ParseRunnerResult Run(ParseRunnerContext context, Token token)
        {
            Debug.Assert(context.Current != null);
            //Debug.Assert(context.PreSignToken == null);

            switch (token)
            {
                case WhiteSpaceToken _:
                    return ParseRunnerResult.Empty(this);
                
                case IdentityToken identity:
                    context.CombineAfter(VariableTerm.Create(identity.Identity));
                    return ParseRunnerResult.Empty(ApplyingRunner.Instance);

                // case NumericToken numeric:
                //     context.CombineAfter(
                //         ParserUtilities.GetNumericConstant(numeric.Value, NumericalSignes.Plus));
                //     return ParseRunnerResult.Empty(this);
                //
                // case NumericalSignToken numericSign:
                //     // "abc -" / "123 -" ==> binary op or signed
                //     if (context.LastToken is WhiteSpaceToken)
                //     {
                //         context.PreSignToken = numericSign;
                //         return ParseRunnerResult.Empty(NumericalSignedRunner.Instance);
                //     }
                //     // "abc-" / "123-" / "(abc)-" ==> binary op
                //     else
                //     {
                //         context.CombineAfter(
                //             new IdentityTerm(numericSign.Symbol.ToString()));
                //         return ParseRunnerResult.Empty(this);
                //     }

                // case OpenParenthesisToken parenthesis:
                //     context.PushScope(parenthesis.Pair);
                //     return ParseRunnerResult.Empty(WaitingRunner.Instance);
                //
                // case CloseParenthesisToken parenthesis:
                //     while (true)
                //     {
                //         var result = ParserUtilities.LeaveOneScope(context, parenthesis.Pair);
                //         Debug.Assert(result != LeaveScopeResults.None);
                //         if (result == LeaveScopeResults.Explicitly)
                //         {
                //             break;
                //         }
                //     }
                //     return ParseRunnerResult.Empty(this);

                default:
                    throw new InvalidOperationException(token.ToString());
            }
        }

        public static readonly ParseRunner Instance = new ApplyingRunner();
    }
}
