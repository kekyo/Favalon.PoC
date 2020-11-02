using System.Diagnostics;
using Favalet.Tokens;

namespace Favalet.Parsers
{
    public abstract class ParseRunner
    {
        [DebuggerStepThrough]
        protected ParseRunner()
        { }

        public abstract ParseRunnerResult Run(ParseRunnerContext context, Token token);
    }
}