using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Favalon.ParseRunners
{
    internal sealed class ParseRunnerContext
    {
        public Term? CurrentTerm;
        public BoundTermPrecedences? CurrentPrecedence;
        public NumericalSignToken? PreSignToken;
        public Token? LastToken;
        public BoundTermAssociatives ApplyNextAssociative;
        public readonly Stack<ScopeInformation> Scopes;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private ParseRunnerContext(Context context, Stack<ScopeInformation> scopes)
        {
            this.CurrentContext = context;
            this.CurrentTerm = null;
            this.CurrentPrecedence = null;
            this.PreSignToken = null;
            this.LastToken = null;
            this.ApplyNextAssociative = BoundTermAssociatives.LeftToRight;
            this.Scopes = scopes;
        }

        public Context CurrentContext { get; private set; }

        public void PushScope(ParenthesisPair? parenthesisPair = null)
        {
            this.Scopes.Push(new ScopeInformation(this.CurrentContext, this.CurrentTerm, this.CurrentPrecedence, parenthesisPair));
            this.CurrentContext = this.CurrentContext.Clone();
            this.CurrentTerm = null;
            this.CurrentPrecedence = null;
        }

        public bool TryPopScope(out ScopeInformation scope)
        {
            if (this.Scopes.Count >= 1)
            {
                scope = this.Scopes.Pop();
                this.CurrentContext = scope.SavedContext;
                return true;
            }
            else
            {
                scope = default;
                return false;
            }
        }

        public override string ToString()
        {
            var currentContext = this.CurrentContext.ToString();
            var currentTerm = this.CurrentTerm?.Readable ?? "[null]";
            var currentPrecedence = this.CurrentPrecedence?.ToString() ?? "[null]";
            var willApplyAssociative = this.ApplyNextAssociative.ToString();
            var scopes = string.Join(",", this.Scopes.Select(scope => $"[{scope}]").ToArray());

            return $"{currentContext},'{currentTerm}', P={currentPrecedence}, {willApplyAssociative}, [{scopes}]";
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParseRunnerContext Create(Context context) =>
            new ParseRunnerContext(context, new Stack<ScopeInformation>());
    }
}
