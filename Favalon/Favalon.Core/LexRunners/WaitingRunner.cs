using Favalon.Tokens;

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
                    if (!char.IsWhiteSpace(ch) && !char.IsControl(ch))
                    {
                        context.TokenBuffer.Append(ch);
                        return RunResult.Empty(IdentityRunner.Instance);
                    }
                    else
                    {
                        return RunResult.Empty(this);
                    }
            }
        }

        public static readonly Runner Instance = new WaitingRunner();
    }
}
