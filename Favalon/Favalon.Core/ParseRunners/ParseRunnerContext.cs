using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private ParseRunnerContext(Context context, Stack<ScopeInformation> scopes)
        {
            this.Context = context;
            this.CurrentTerm = null;
            this.Scopes = scopes;
            this.PreSignToken = null;
            this.LastToken = null;
            this.WillApplyRightToLeft = false;
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParseRunnerContext Create(Context context) =>
            new ParseRunnerContext(context, new Stack<ScopeInformation>());
    }
}
