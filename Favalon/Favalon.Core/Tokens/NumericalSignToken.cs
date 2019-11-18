using System;

namespace Favalon.Tokens
{
    public enum Signs
    {
        Plus = 1,
        Minus = -1
    }

    public sealed class NumericalSignToken :
        Token, IEquatable<NumericalSignToken>
    {
        public readonly Signs Sign;

        private NumericalSignToken(Signs sign) =>
            this.Sign = sign;

        public override int GetHashCode() =>
            this.Sign.GetHashCode();

        public bool Equals(NumericalSignToken? other) =>
            other?.Sign.Equals(this.Sign) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as NumericalSignToken);

        public override string ToString() =>
            this.Sign switch
            {
                Signs.Plus => "+",
                Signs.Minus => "-",
                _ => throw new InvalidOperationException()
            };

        public void Deconstruct(out Signs sign) =>
            sign = this.Sign;

        internal static readonly NumericalSignToken Plus =
            new NumericalSignToken(Signs.Plus);
        internal static readonly NumericalSignToken Minus =
            new NumericalSignToken(Signs.Minus);
    }
}
