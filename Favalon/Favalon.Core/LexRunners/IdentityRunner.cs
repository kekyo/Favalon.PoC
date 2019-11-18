using Favalon.Internal;
using Favalon.Tokens;
using System;

namespace Favalon.LexRunners
{
    internal sealed class IdentityRunner : LexRunner
    {
        private IdentityRunner()
        { }

        private static IdentityToken InternalFinish(RunContext context)
        {
            var token = context.TokenBuffer.ToString();
            context.TokenBuffer.Clear();
            return new IdentityToken(token);
        }

        public override RunResult Run(RunContext context, char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                var token = context.TokenBuffer.ToString();
                context.TokenBuffer.Clear();
                return RunResult.Create(WaitingIgnoreSpaceRunner.Instance, new IdentityToken(token), WhiteSpaceToken.Instance);
            }
            else if (Characters.IsOpenParenthesis(ch) is ParenthesisInformation)
            {
                return RunResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context),
                    Token.Open(ch));
            }
            else if (Characters.IsCloseParenthesis(ch) is ParenthesisInformation)
            {
                return RunResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context),
                    Token.Close(ch));
            }
            else if (Characters.IsOperator(ch))
            {
                var token0 = InternalFinish(context);
                context.TokenBuffer.Append(ch);
                return RunResult.Create(OperatorRunner.Instance, token0);
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

        public override RunResult Finish(RunContext context) =>
            RunResult.Create(WaitingRunner.Instance, InternalFinish(context));

        public static readonly LexRunner Instance = new IdentityRunner();
    }
}
