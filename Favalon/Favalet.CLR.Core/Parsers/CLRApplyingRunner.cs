using Favalet.Expressions;
using Favalet.Tokens;
using System;
using System.Diagnostics;

namespace Favalet.Parsers
{
    internal sealed class CLRApplyingRunner : ApplyingRunner
    {
        [DebuggerStepThrough]
        private CLRApplyingRunner()
        { }

        public override ParseRunnerResult Run(ParseRunnerContext context, Token token)
        {
            Debug.Assert(context.Current != null);
            //Debug.Assert(context.PreSignToken == null);

            switch (token)
            {
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

                default:
                    return base.Run(context, token);
            }
        }

        public new static readonly ParseRunner Instance =
            new CLRApplyingRunner();
    }
}
