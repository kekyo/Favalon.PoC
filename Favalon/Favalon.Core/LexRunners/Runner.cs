using Favalon.Tokens;

namespace Favalon.LexRunners
{
    internal abstract class Runner
    {
        protected Runner()
        { }

        public abstract RunResult Run(RunContext context, char ch);

        public virtual RunResult Finish(RunContext context) =>
            RunResult.Empty(this);
    }
}
