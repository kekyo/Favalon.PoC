using Favalon.Internal;
using Favalon.Tokens;
using System;

namespace Favalon.LexRunners
{
    internal sealed class WaitingIgnoreSpaceRunner : LexRunner
    {
        private WaitingIgnoreSpaceRunner()
        { }

        public override RunResult Run(RunContext context, char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                return RunResult.Empty(this);
            }
            else if (char.IsDigit(ch))
            {
                context.TokenBuffer.Append(ch);
                return RunResult.Empty(NumericRunner.Instance);
            }
            else if (Characters.IsOpenParenthesis(ch) is ParenthesisInformation)
            {
                return RunResult.Create(
                    WaitingRunner.Instance,
                    Token.Open(ch));
            }
            else if (Characters.IsCloseParenthesis(ch) is ParenthesisInformation)
            {
                return RunResult.Create(
                    WaitingRunner.Instance,
                    Token.Close(ch));
            }
            else if (Characters.IsOperator(ch))
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

        public static readonly LexRunner Instance = new WaitingIgnoreSpaceRunner();
    }
}
