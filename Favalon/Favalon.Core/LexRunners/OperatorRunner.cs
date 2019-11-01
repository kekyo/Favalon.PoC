using Favalon.Internal;
using Favalon.Tokens;
using System;

namespace Favalon.LexRunners
{
    internal sealed class OperatorRunner : Runner
    {
        private OperatorRunner()
        { }

        private static IdentityToken InternalFinish(RunContext context)
        {
            var token = context.TokenBuffer.ToString();
            context.TokenBuffer.Clear();
            return new IdentityToken(token);
        }

        public override RunResult Run(RunContext context, char ch)
        {
            switch (ch)
            {
                case '(':
                    return RunResult.Create(WaitingRunner.Instance, InternalFinish(context), BeginBracketToken.Instance);
                case ')':
                    return RunResult.Create(WaitingRunner.Instance, InternalFinish(context), EndBracketToken.Instance);
                default:
                    if (char.IsWhiteSpace(ch))
                    {
                        var token = context.TokenBuffer.ToString();
                        context.TokenBuffer.Clear();
                        return RunResult.Create(WaitingIgnoreSpaceRunner.Instance, new IdentityToken(token), WhiteSpaceToken.Instance);
                    }
                    else if (char.IsDigit(ch))
                    {
                        var token0 = InternalFinish(context);
                        context.TokenBuffer.Append(ch);
                        return RunResult.Create(NumericRunner.Instance, token0);
                    }
                    else if (!Utilities.IsOperator(ch))
                    {
                        var token0 = InternalFinish(context);
                        context.TokenBuffer.Append(ch);
                        return RunResult.Create(IdentityRunner.Instance, token0);
                    }
                    else if (!char.IsControl(ch))
                    {
                        context.TokenBuffer.Append(ch);
                        return RunResult.Empty(this);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
            }
        }

        public override RunResult Finish(RunContext context) =>
            RunResult.Create(WaitingRunner.Instance, InternalFinish(context));

        public static readonly Runner Instance = new OperatorRunner();
    }
}
