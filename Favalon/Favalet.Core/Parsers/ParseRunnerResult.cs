using Favalet.Expressions;
using System.Runtime.CompilerServices;

namespace Favalet.Parsers
{
    internal struct ParseRunnerResult
    {
        public readonly ParseRunner Next;
        public readonly IExpression? Term;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private ParseRunnerResult(ParseRunner next, IExpression? term)
        {
            this.Next = next;
            this.Term = term;
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParseRunnerResult Empty(ParseRunner next) =>
            new ParseRunnerResult(next, null);

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParseRunnerResult Create(ParseRunner next, IExpression? term) =>
            new ParseRunnerResult(next, term);

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Deconstruct(out ParseRunner next, out IExpression? term)
        {
            next = this.Next;
            term = this.Term;
        }
    }
}
