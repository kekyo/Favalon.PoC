using System;

namespace Favalon.Tokens
{
    public abstract class ParenthesisToken :
        Token, IEquatable<ParenthesisToken?>
    {
        public readonly char Symbol;

        internal ParenthesisToken(char symbol) =>
            this.Symbol = symbol;

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(ParenthesisToken? other) =>
            other?.Symbol.Equals(this.Symbol) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as ParenthesisToken);

        public override string ToString() =>
            this.Symbol.ToString();

        public void Deconstruct(out char symbol) =>
            symbol = this.Symbol;
    }
}
