namespace Favalon.Contexts
{
    public struct UnifyResult
    {
        public readonly bool Unified;
        public readonly Term Term;

        internal UnifyResult(bool unified, Term term)
        {
            this.Unified = unified;
            this.Term = term;
        }

        public void Deconstruct(out bool unified, out Term term)
        {
            unified = this.Unified;
            term = this.Term;
        }
    }
}
