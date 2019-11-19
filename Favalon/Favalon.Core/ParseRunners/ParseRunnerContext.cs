using Favalon.Terms;
using System.Collections.Generic;

namespace Favalon.ParseRunners
{
    internal sealed class ParseRunnerContext
    {
        public readonly Context Context;
        public Term? CurrentTerm;
        public readonly Stack<ParenthesisScope> ParenthesisScopes;

        private ParseRunnerContext(Context context, Stack<ParenthesisScope> parenthesisScopes)
        {
            this.Context = context;
            this.CurrentTerm = null;
            this.ParenthesisScopes = parenthesisScopes;
        }

        public static ParseRunnerContext Create(Context context) =>
            new ParseRunnerContext(context, new Stack<ParenthesisScope>());
    }
}
