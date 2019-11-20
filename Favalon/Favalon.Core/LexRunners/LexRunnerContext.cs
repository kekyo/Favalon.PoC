using System.Text;

namespace Favalon.LexRunners
{
    internal sealed class LexRunnerContext
    {
        public readonly StringBuilder TokenBuffer;

        private LexRunnerContext(StringBuilder tokenBuffer) =>
            this.TokenBuffer = tokenBuffer;

        public static LexRunnerContext Create() =>
            new LexRunnerContext(new StringBuilder());
    }
}
