using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Favalon.ParseRunners
{
    internal sealed class ParseRunnerContext
    {
        public readonly Context Context;
        public Term? CurrentTerm;
        public BoundTermPrecedences? CurrentPrecedence;
        public NumericalSignToken? PreSignToken;
        public Token? LastToken;
        public bool WillApplyRightToLeft;
        public readonly Stack<ScopeInformation> Scopes;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private ParseRunnerContext(Context context, Stack<ScopeInformation> scopes)
        {
            this.Context = context;
            this.CurrentTerm = null;
            this.CurrentPrecedence = null;
            this.PreSignToken = null;
            this.LastToken = null;
            this.WillApplyRightToLeft = false;
            this.Scopes = scopes;
        }

        public void PushScope(ParenthesisPair? parenthesisPair = null)
        {
            this.Scopes.Push(new ScopeInformation(this.CurrentTerm, this.CurrentPrecedence, parenthesisPair));
            this.CurrentTerm = null;
            this.CurrentPrecedence = null;
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParseRunnerContext Create(Context context) =>
            new ParseRunnerContext(context, new Stack<ScopeInformation>());
    }
}
