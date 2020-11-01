namespace Favalet.LexRunners
{
    internal abstract class LexRunner
    {
        protected LexRunner()
        { }

        public abstract LexRunnerResult Run(LexRunnerContext context, char ch);

        public virtual LexRunnerResult Finish(LexRunnerContext context) =>
            LexRunnerResult.Empty(this);
    }
}
