using Favalon.Terms;

namespace Favalon.ParseRunners
{
    internal sealed class ParseRunnerContext
    {
        public readonly Context Context;
        public Term? CurrentTerm;

        private ParseRunnerContext(Context context)
        {
            this.Context = context;
            this.CurrentTerm = null;
        }

        public static ParseRunnerContext Create(Context context) =>
            new ParseRunnerContext(context);
    }
}
