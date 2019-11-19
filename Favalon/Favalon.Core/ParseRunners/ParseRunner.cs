using Favalon.Terms;
using Favalon.Tokens;

namespace Favalon.ParseRunners
{
    internal abstract class ParseRunner
    {
        protected ParseRunner()
        { }

        public abstract ParseRunnerResult Run(ParseRunnerContext context, Token token);

        public virtual ParseRunnerResult Finish(ParseRunnerContext context) =>
            ParseRunnerResult.Create(WaitingRunner.Instance, context.CurrentTerm);
    }
}
