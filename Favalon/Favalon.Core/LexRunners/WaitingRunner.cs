using Favalon.Tokens;
using System;

namespace Favalon.LexRunners
{
    internal sealed class WaitingRunner : Runner
    {
        private WaitingRunner()
        { }

        public override RunResult Run(RunContext context, char ch)
        {
            switch (ch)
            {
                case '(':
                    return RunResult.Create(this, BeginBracketToken.Instance);
                case ')':
                    return RunResult.Create(this, EndBracketToken.Instance);
                default:
                    if (char.IsWhiteSpace(ch))
                    {
                        return RunResult.Create(WaitingIgnoreSpaceRunner.Instance, WhiteSpaceToken.Instance);
                    }
                    else if (char.IsDigit(ch))
                    {
                        context.TokenBuffer.Append(ch);
                        return RunResult.Empty(NumericRunner.Instance);
                    }
                    else if (Utilities.IsOperator(ch))
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

        public static readonly Runner Instance = new WaitingRunner();
    }
}
