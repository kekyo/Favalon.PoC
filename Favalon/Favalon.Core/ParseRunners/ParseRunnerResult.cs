using Favalon.Terms;
using System.Runtime.CompilerServices;

namespace Favalon.ParseRunners
{
    internal struct ParseRunnerResult
    {
        public readonly ParseRunner Next;
        public readonly Term? Term;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private ParseRunnerResult(ParseRunner next, Term? term)
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
        public static ParseRunnerResult Create(ParseRunner next, Term? term) =>
            new ParseRunnerResult(next, term);

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Deconstruct(out ParseRunner next, out Term? term)
        {
            next = this.Next;
            term = this.Term;
        }
    }
}
