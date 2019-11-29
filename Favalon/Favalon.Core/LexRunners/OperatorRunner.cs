using Favalon.Internal;
using Favalon.Tokens;
using System;

namespace Favalon.LexRunners
{
    internal sealed class OperatorRunner : LexRunner
    {
        private OperatorRunner()
        { }

        private static Token InternalFinish(LexRunnerContext context, bool forceIdentity)
        {
            var token = context.TokenBuffer.ToString();
            context.TokenBuffer.Clear();
            if (!forceIdentity && (token.Length == 1) &&
                StringUtilities.IsNumericSign(token[0]) is NumericalSignes sign)
            {
                return (sign == NumericalSignes.Plus) ?
                    NumericalSignToken.Plus : NumericalSignToken.Minus;
            }
            else
            {
                return new IdentityToken(token);
            }
        }

        public override LexRunnerResult Run(LexRunnerContext context, char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                var token0 = InternalFinish(context, true);
                context.TokenBuffer.Clear();
                return LexRunnerResult.Create(WaitingIgnoreSpaceRunner.Instance, token0, WhiteSpaceToken.Instance);
            }
            else if (char.IsDigit(ch))
            {
                var token0 = InternalFinish(context, false);
                context.TokenBuffer.Append(ch);
                return LexRunnerResult.Create(NumericRunner.Instance, token0);
            }
            else if (StringUtilities.IsOpenParenthesis(ch) is ParenthesisPair)
            {
                return LexRunnerResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context, true),
                    Token.Open(ch));
            }
            else if (StringUtilities.IsCloseParenthesis(ch) is ParenthesisPair)
            {
                return LexRunnerResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context, true),
                    Token.Close(ch));
            }
            else if (StringUtilities.IsOperator(ch))
            {
                context.TokenBuffer.Append(ch);
                return LexRunnerResult.Empty(this);
            }
            else if(!char.IsControl(ch))
            {
                var token0 = InternalFinish(context, true);
                context.TokenBuffer.Append(ch);
                return LexRunnerResult.Create(IdentityRunner.Instance, token0);
            }
            else
            {
                throw new InvalidOperationException(ch.ToString());
            }
        }

        public override LexRunnerResult Finish(LexRunnerContext context) =>
            LexRunnerResult.Create(WaitingRunner.Instance, InternalFinish(context, true));

        public static readonly LexRunner Instance = new OperatorRunner();
    }
}
