using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;

namespace Favalon.ParseRunners
{
    internal sealed class ParseRunnerContext
    {
        public readonly Context Context;
        public Term? CurrentTerm;
        public readonly Stack<ScopeInformation> Scopes;
        public NumericalSignToken? PreSignToken;
        public Token? LastToken;
        public bool WillApplyRightToLeft;

        private ParseRunnerContext(Context context, Stack<ScopeInformation> scopes)
        {
            this.Context = context;
            this.CurrentTerm = null;
            this.Scopes = scopes;
            this.PreSignToken = null;
            this.LastToken = null;
            this.WillApplyRightToLeft = false;
        }

        public static ParseRunnerContext Create(Context context) =>
            new ParseRunnerContext(context, new Stack<ScopeInformation>());
    }
}
