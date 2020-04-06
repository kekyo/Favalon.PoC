using Favalet.Tokens;

namespace Favalet.Parsers.Runners
{
    internal abstract class ParseRunner
    {
        protected ParseRunner()
        { }

        public abstract ParseRunner Run(ParseRunnerContext context, Token token);
    }
}
