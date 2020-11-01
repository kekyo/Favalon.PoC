using Favalet.Expressions;
using Favalet.Tokens;
using System.Runtime.CompilerServices;

namespace Favalet.Parsers
{
    internal struct ScopeInformation
    {
        public readonly Context SavedContext;
        public readonly IExpression? SavedTerm;
        public readonly BoundTermPrecedences? Precedence;
        public readonly ParenthesisPair? ParenthesisPair;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ScopeInformation(
            Context savedContext,
            IExpression? savedTerm,
            BoundTermPrecedences? precedence,
            ParenthesisPair? parenthesisPair)
        {
            this.SavedContext = savedContext;
            this.SavedTerm = savedTerm;
            this.Precedence = precedence;
            this.ParenthesisPair = parenthesisPair;
        }

        public override string ToString()
        {
            var savedContext = this.SavedContext?.ToString();
            var savedTerm = this.SavedTerm?.Readable ?? "[null]";
            var precedence = this.Precedence?.ToString() ?? "[null]";
            return $"{savedContext},'{savedTerm}',P={precedence},PP={this.ParenthesisPair}";
        }
    }
}
