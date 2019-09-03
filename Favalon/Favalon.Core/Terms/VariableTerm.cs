namespace Favalon.Terms
{
    public class VariableTerm : Term
    {
        public readonly string SymbolName;

        public VariableTerm(string symbolName) =>
            this.SymbolName = symbolName;

        public bool Equals(VariableTerm? rhs) =>
            rhs is VariableTerm term ? this.SymbolName.Equals(term.SymbolName) : false;

        public override bool Equals(Term? other) =>
            this.Equals(other as VariableTerm);

        public override string ToString() =>
            this.SymbolName;
    }

    public class VariableTerm<TTerm> : VariableTerm
        where TTerm : VariableTerm<TTerm>
    {
        protected VariableTerm(string symbolName) :
            base(symbolName)
        { }

        public bool Equals(TTerm? rhs) =>
            rhs is TTerm term ? this.SymbolName.Equals(term.SymbolName) : false;
    }
}
