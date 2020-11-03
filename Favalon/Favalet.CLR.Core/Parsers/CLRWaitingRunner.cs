using Favalet.Expressions;
using Favalet.Tokens;
using System;
using System.Diagnostics;

namespace Favalet.Parsers
{
    internal sealed class CLRWaitingRunner : WaitingRunner
    {
        [DebuggerStepThrough]
        private CLRWaitingRunner()
        { }

        public override ParseRunnerResult Run(ParseRunnerContext context, Token token)
        {
            Debug.Assert(context.Current == null);
            //Debug.Assert(context.PreSignToken == null);

            switch (token)
            {
                // "123"
                case NumericToken numeric:
                    if (int.TryParse(numeric.Value, out var intValue))
                    {
                        context.CombineAfter(ConstantTerm.From(intValue));
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Couldn't parse numeric: {numeric.Value}");
                    }
                    //context.CombineAfter(
                    //    ParserUtilities.GetNumericConstant(numeric.Value, NumericalSignes.Plus));
                    return ParseRunnerResult.Empty(ApplyingRunner.Instance);
                
                // // "-"
                // case NumericalSignToken numericSign:
                //     context.PreSignToken = numericSign;
                //     return ParseRunnerResult.Empty(NumericalSignedRunner.Instance);

                default:
                    return base.Run(context, token);
            }
        }

        public new static readonly ParseRunner Instance =
            new CLRWaitingRunner();
    }
}
