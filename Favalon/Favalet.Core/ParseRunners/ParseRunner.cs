using Favalet.Tokens;

namespace Favalet.ParseRunners
{
    internal abstract class ParseRunner
    {
        protected ParseRunner()
        { }

        public abstract ParseRunner Run(ParseRunnerContext context, Token token);
    }
}
