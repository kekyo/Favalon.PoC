namespace Favalon.Terms
{
    public struct BoundTermInformation
    {
        public readonly bool Infix;
        public readonly bool RightToLeft;
        public readonly Term Term;

        internal BoundTermInformation(bool infix, bool rightToLeft, Term term)
        {
            this.Infix = infix;
            this.RightToLeft = rightToLeft;
            this.Term = term;
        }

        public void Deconstruct(out bool infix, out bool rightToLeft, out Term term)
        {
            infix = this.Infix;
            rightToLeft = this.RightToLeft;
            term = this.Term;
        }
    }
}
