namespace Favalon.Terms
{
    public sealed class ApplyTerm : VariableTerm<ApplyTerm>
    {
        public readonly Term Rhs;

        public ApplyTerm(string symbolName, Term rhs) :
            base(symbolName) =>
            this.Rhs = rhs;

        public override string ToString() =>
            $"{this.SymbolName} {this.Rhs}";
    }
}
