using Favalon.Internal;
using Favalon.Tokens;
using System;

namespace Favalon.LexRunners
{
    internal sealed class OperatorRunner : Runner
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
            switch (ch)
            {
                case '(':
                    return RunResult.Create(
                        WaitingRunner.Instance,
                        InternalFinish(context, true),
                        Token.open);
                case ')':
                    return RunResult.Create(
                        WaitingRunner.Instance,
                        InternalFinish(context, true),
                        Token.close);
                default:
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
        }

        public override RunResult Finish(RunContext context) =>
            RunResult.Create(WaitingRunner.Instance, InternalFinish(context, true));

        public static readonly Runner Instance = new OperatorRunner();
    }
}
