using System.Text;

namespace Favalon.LexRunners
{
    internal struct RunContext
    {
        public readonly StringBuilder TokenBuffer;

        private RunContext(StringBuilder tokenBuffer) =>
            this.TokenBuffer = tokenBuffer;

        public static RunContext Create() =>
            new RunContext(new StringBuilder());
    }
}
