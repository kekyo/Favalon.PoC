using Favalon.Tokens;

namespace Favalon.LexRunners
{
    internal sealed class IdentityRunner : Runner
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
            switch (ch)
            {
                case '(':
                    return RunResult.Create(WaitingRunner.Instance, InternalFinish(context), BeginBracketToken.Instance);
                case ')':
                    return RunResult.Create(WaitingRunner.Instance, InternalFinish(context),  EndBracketToken.Instance);
                default:
                    if (char.IsWhiteSpace(ch))
                    {
                        var token = context.TokenBuffer.ToString();
                        context.TokenBuffer.Clear();
                        return RunResult.Create(WaitingRunner.Instance, new IdentityToken(token));
                    }
                    else
                    {
                        context.TokenBuffer.Append(ch);
                        return RunResult.Empty(this);
                    }
            }
        }

        public override RunResult Finish(RunContext context) =>
            RunResult.Create(WaitingRunner.Instance, InternalFinish(context));

        public static readonly Runner Instance = new IdentityRunner();
    }
}
