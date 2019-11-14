using Favalon.Internal;
using Favalon.Tokens;
using System;

namespace Favalon.LexRunners
{
    internal sealed class WaitingIgnoreSpaceRunner : Runner
    {
        private WaitingIgnoreSpaceRunner()
        { }

        public override RunResult Run(RunContext context, char ch)
        {
            switch (ch)
            {
                case '(':
                    return RunResult.Create(
                        WaitingRunner.Instance,
                        OperatorToken.Open);
                case ')':
                    return RunResult.Create(
                        WaitingRunner.Instance,
                        OperatorToken.Close);
                default:
                    if (char.IsWhiteSpace(ch))
                    {
                        return RunResult.Empty(this);
                    }
                    else if (char.IsDigit(ch))
                    {
                        context.TokenBuffer.Append(ch);
                        return RunResult.Empty(NumericRunner.Instance);
                    }
                    else if (IsOperator(ch))
                    {
                        context.TokenBuffer.Append(ch);
                        return RunResult.Empty(OperatorRunner.Instance);
                    }
                    else if (!char.IsControl(ch))
                    {
                        context.TokenBuffer.Append(ch);
                        return RunResult.Empty(IdentityRunner.Instance);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
            }
        }

        public static readonly Runner Instance = new WaitingIgnoreSpaceRunner();
    }
}
