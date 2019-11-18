namespace Favalon.LexRunners
{
    internal abstract class LexRunner
    {
        protected LexRunner()
        { }

        public abstract RunResult Run(RunContext context, char ch);

        public virtual RunResult Finish(RunContext context) =>
            RunResult.Empty(this);
    }
}
