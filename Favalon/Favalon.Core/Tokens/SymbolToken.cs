using System;

namespace Favalon.Tokens
{
    public abstract class SymbolToken :
        Token, IEquatable<SymbolToken?>
    {
        public readonly char Symbol;

        internal SymbolToken(char symbol) =>
            this.Symbol = symbol;

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(SymbolToken? other) =>
            other?.Symbol.Equals(this.Symbol) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as SymbolToken);

        public override string ToString() =>
            this.Symbol.ToString();

        public void Deconstruct(out char symbol) =>
            symbol = this.Symbol;
    }
}
