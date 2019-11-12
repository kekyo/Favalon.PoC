using System.Collections.Generic;

namespace Favalon.LexRunners
{
    internal abstract class Runner
    {
        internal static readonly HashSet<char> operatorChars = new HashSet<char>
        {
            '!'/* , '"' */, '#', '$', '%', '&' /* , ''' */, /* '(', ')', */
            '*', '+', ',', '-'/* , '.'*/, '/'/*, ':' */, ';', '<', '=', '>', '?',
            '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~'
        };

        public static bool IsOperator(char ch) =>
            operatorChars.Contains(ch);

        protected Runner()
        { }

        public abstract RunResult Run(RunContext context, char ch);

        public virtual RunResult Finish(RunContext context) =>
            RunResult.Empty(this);
    }
}
