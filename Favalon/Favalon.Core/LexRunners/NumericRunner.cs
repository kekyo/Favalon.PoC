using Favalon.Internal;
using Favalon.Tokens;
using System;

namespace Favalon.LexRunners
{
    internal sealed class NumericRunner : LexRunner
    {
        private NumericRunner()
        { }

        private static NumericToken InternalFinish(LexRunnerContext context)
        {
            var token = context.TokenBuffer.ToString();
            context.TokenBuffer.Clear();
            return new NumericToken(token);
        }

        public override LexRunnerResult Run(LexRunnerContext context, char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                var token = context.TokenBuffer.ToString();
                context.TokenBuffer.Clear();
                return LexRunnerResult.Create(
                    WaitingIgnoreSpaceRunner.Instance,
                    new NumericToken(token),
                    WhiteSpaceToken.Instance);
            }
            else if (Characters.IsOpenParenthesis(ch) is ParenthesisPair)
            {
                return LexRunnerResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context),
                    Token.Open(ch));
            }
            else if (Characters.IsCloseParenthesis(ch) is ParenthesisPair)
            {
                return LexRunnerResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context),
                    Token.Close(ch));
            }
            else if (Characters.IsOperator(ch))
            {
                var token0 = InternalFinish(context);
                context.TokenBuffer.Append(ch);
                return LexRunnerResult.Create(OperatorRunner.Instance, token0);
            }
            else if (char.IsDigit(ch))
            {
                context.TokenBuffer.Append(ch);
                return LexRunnerResult.Empty(this);
            }
            else
            {
                throw new InvalidOperationException(ch.ToString());
            }
        }

        public override LexRunnerResult Finish(LexRunnerContext context) =>
            LexRunnerResult.Create(WaitingRunner.Instance, InternalFinish(context));

        public static readonly LexRunner Instance = new NumericRunner();
    }
}
