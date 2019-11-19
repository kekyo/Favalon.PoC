namespace Favalon.Tokens
{
    public enum Signes
    {
        Unknown = 0,
        Plus = 1,
        Minus = -1
    }

    public sealed class NumericalSignToken :
        SymbolToken
    {
        public readonly Signes Sign;

        private NumericalSignToken(Signes sign) =>
            this.Sign = sign;

        public override char Symbol =>
            this.Sign switch
            {
                Signes.Plus => '+',
                Signes.Minus => '-',
                _ => '?'
            };

        public override int GetHashCode() =>
            this.Sign.GetHashCode();

        public bool Equals(NumericalSignToken? other) =>
            other?.Sign.Equals(this.Sign) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as NumericalSignToken);

        public void Deconstruct(out Signes sign) =>
            sign = this.Sign;

        internal static readonly NumericalSignToken Plus =
            new NumericalSignToken(Signes.Plus);
        internal static readonly NumericalSignToken Minus =
            new NumericalSignToken(Signes.Minus);
    }
}
