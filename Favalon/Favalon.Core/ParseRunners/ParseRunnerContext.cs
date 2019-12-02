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
        public NumericalSignToken? PreSignToken;
        public BoundTermAssociatives ApplyNextAssociative;

        private readonly Stack<ScopeInformation> scopes;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private ParseRunnerContext(Context context, Stack<ScopeInformation> scopes)
        {
            this.CurrentContext = context;
            this.PreSignToken = null;
            this.LastToken = null;
            this.ApplyNextAssociative = BoundTermAssociatives.LeftToRight;
            this.scopes = scopes;
        }

        public Context CurrentContext { get; private set; }
        public Term? CurrentTerm { get; private set; }
        public BoundTermPrecedences? CurrentPrecedence { get; private set; }
        public Token? LastToken { get; private set; }

        public void SetLastToken(Token token) =>
            this.LastToken = token;

        public void SetTerm(Term term) =>
            this.CurrentTerm = term;
        public void CombineAfter(Term term) =>
            this.CurrentTerm = ParserUtilities.CombineTerms(this.CurrentTerm, term);
        public void CombineBefore(Term term) =>
            this.CurrentTerm = ParserUtilities.CombineTerms(term, this.CurrentTerm);

        public void MakeHidedApplyTerm() =>
            this.CurrentTerm = ParserUtilities.HideTerm(this.CurrentTerm);

        public void SetPrecedence(BoundTermPrecedences precedence) =>
            this.CurrentPrecedence = precedence;

        public void PushScope(ParenthesisPair? parenthesisPair = null)
        {
            this.scopes.Push(new ScopeInformation(this.CurrentContext, this.CurrentTerm, this.CurrentPrecedence, parenthesisPair));
            this.CurrentContext = this.CurrentContext.Clone();
            this.CurrentTerm = null;
            this.CurrentPrecedence = null;
        }

        public bool TryPopScope(out ParenthesisPair? parenthesisPair)
        {
            if (this.scopes.Count >= 1)
            {
                var scope = this.scopes.Pop();

                // Retreive the context.
                this.CurrentContext = scope.SavedContext;

                // Make term hiding:
                // because invalid deconstruction ApplyTerm for next token iteration.
                var hideTerm = ParserUtilities.HideTerm(this.CurrentTerm);

                // Combine it implicitly.
                this.CurrentTerm = ParserUtilities.CombineTerms(
                    scope.SavedTerm,
                    hideTerm);

                // Reset precedence, because finished a scope.
                this.CurrentPrecedence = null;

                parenthesisPair = scope.ParenthesisPair;

                return true;
            }
            else
            {
                parenthesisPair = default;
                return false;
            }
        }

        public override string ToString()
        {
            var currentContext = this.CurrentContext.ToString();
            var currentTerm = this.CurrentTerm?.Readable ?? "[null]";
            var currentPrecedence = this.CurrentPrecedence?.ToString() ?? "[null]";
            var willApplyAssociative = this.ApplyNextAssociative.ToString();
            var scopes = string.Join(",", this.scopes.Select(scope => $"[{scope}]").ToArray());

            return $"{currentContext},'{currentTerm}', P={currentPrecedence}, {willApplyAssociative}, [{scopes}]";
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParseRunnerContext Create(Context context) =>
            new ParseRunnerContext(context, new Stack<ScopeInformation>());
    }
}
