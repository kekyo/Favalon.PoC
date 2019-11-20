using Favalon.Terms;

namespace Favalon.ParseRunners
{
    internal struct ParseRunnerResult
    {
        public readonly ParseRunner Next;
        public readonly Term? Term;

        private ParseRunnerResult(ParseRunner next, Term? term)
        {
            this.Next = next;
            this.Term = term;
        }

        public static ParseRunnerResult Empty(ParseRunner next) =>
            new ParseRunnerResult(next, null);
        public static ParseRunnerResult Create(ParseRunner next, Term? term) =>
            new ParseRunnerResult(next, term);

        public void Deconstruct(out ParseRunner next, out Term? term)
        {
            next = this.Next;
            term = this.Term;
        }
    }
}
