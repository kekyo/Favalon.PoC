using Favalet.Tokens;

namespace Favalet.Parsers
{
    internal abstract class ParseRunner
    {
        protected ParseRunner()
        { }

        public abstract ParseRunnerResult Run(ParseRunnerContext context, Token token);
    }
}
