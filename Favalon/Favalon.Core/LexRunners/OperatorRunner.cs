using Favalon.Internal;
using Favalon.Tokens;
using System;

namespace Favalon.LexRunners
{
    internal sealed class OperatorRunner : LexRunner
    {
        private OperatorRunner()
        { }

        private static Token InternalFinish(RunContext context, bool forceIdentity)
        {
            var token = context.TokenBuffer.ToString();
            context.TokenBuffer.Clear();
            if (!forceIdentity && (token.Length == 1) && IsNumericSign(token[0]))
            {
                return new NumericalSignToken(token[0]);
            }
            else
            {
                return new IdentityToken(token);
            }
        }

        public override RunResult Run(RunContext context, char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                var token0 = InternalFinish(context, true);
                context.TokenBuffer.Clear();
                return RunResult.Create(WaitingIgnoreSpaceRunner.Instance, token0, WhiteSpaceToken.Instance);
            }
            else if (char.IsDigit(ch))
            {
                var token0 = InternalFinish(context, false);
                context.TokenBuffer.Append(ch);
                return RunResult.Create(NumericRunner.Instance, token0);
            }
            else if (IsOpenParenthesis(ch) is Parenthesis)
            {
                return RunResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context, true),
                    Token.Open(ch));
            }
            else if (IsCloseParenthesis(ch) is Parenthesis)
            {
                return RunResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context, true),
                    Token.Close(ch));
            }
            else if (IsOperator(ch))
            {
                context.TokenBuffer.Append(ch);
                return RunResult.Empty(this);
            }
            else if(!char.IsControl(ch))
            {
                var token0 = InternalFinish(context, true);
                context.TokenBuffer.Append(ch);
                return RunResult.Create(IdentityRunner.Instance, token0);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override RunResult Finish(RunContext context) =>
            RunResult.Create(WaitingRunner.Instance, InternalFinish(context, true));

        public static readonly LexRunner Instance = new OperatorRunner();
    }
}
