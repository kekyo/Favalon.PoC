using System;

namespace Favalon.Tokens
{
    public sealed class OperatorToken :
        Token, IEquatable<OperatorToken?>
    {
        public readonly string Symbol;

        internal OperatorToken(string symbol) =>
            this.Symbol = symbol;

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(OperatorToken? other) =>
            other?.Symbol.Equals(this.Symbol) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as OperatorToken);

        public override string ToString() =>
            this.Symbol;

        public void Deconstruct(out string identity) =>
            identity = this.Symbol;
    }
}
