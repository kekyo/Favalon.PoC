using System;

namespace Favalon.Tokens
{
    public sealed class NumericalSignToken :
        Token, IEquatable<NumericalSignToken?>
    {
        public readonly char Symbol;

        internal NumericalSignToken(char symbol) =>
            this.Symbol = symbol;

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(NumericalSignToken? other) =>
            other?.Symbol.Equals(this.Symbol) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as NumericalSignToken);

        public override string ToString() =>
            this.Symbol.ToString();

        public void Deconstruct(out char symbol) =>
            symbol = this.Symbol;
    }
}
