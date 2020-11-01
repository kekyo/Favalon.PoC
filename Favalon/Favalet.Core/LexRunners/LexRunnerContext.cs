using System.Runtime.CompilerServices;
using System.Text;

namespace Favalet.LexRunners
{
    internal sealed class LexRunnerContext
    {
        public readonly StringBuilder TokenBuffer;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private LexRunnerContext(StringBuilder tokenBuffer) =>
            this.TokenBuffer = tokenBuffer;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static LexRunnerContext Create() =>
            new LexRunnerContext(new StringBuilder());
    }
}
