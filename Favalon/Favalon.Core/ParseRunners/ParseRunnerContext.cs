using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;

namespace Favalon.ParseRunners
{
    internal sealed class ParseRunnerContext
    {
        public readonly Context Context;
        public Term? CurrentTerm;
        public readonly Stack<ParenthesisScope> ParenthesisScopes;
        public NumericalSignToken? PreSignToken;
        public Token? LastToken;

        private ParseRunnerContext(Context context, Stack<ParenthesisScope> parenthesisScopes)
        {
            this.Context = context;
            this.CurrentTerm = null;
            this.ParenthesisScopes = parenthesisScopes;
            this.PreSignToken = null;
            this.LastToken = null;
        }

        public static ParseRunnerContext Create(Context context) =>
            new ParseRunnerContext(context, new Stack<ParenthesisScope>());
    }
}
