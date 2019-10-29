namespace Favalon.Tokens
{
    partial class Token
    {
        public static IdentityToken Identity(string identity) =>
            new IdentityToken(identity);

        public static BeginBracketToken Begin() =>
            BeginBracketToken.Instance;

        public static EndBracketToken End() =>
            EndBracketToken.Instance;

        public static NumericToken Numeric(int value) =>
            new NumericToken(value.ToString());
    }
}
