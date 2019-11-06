namespace Favalon.Tokens
{
    partial class Token
    {
        public static IdentityToken Identity(string identity) =>
            new IdentityToken(identity);

        public static OperatorToken Operator(string symbol) =>
            new OperatorToken(symbol);

        public static OperatorToken Open() =>
            OperatorToken.Open;

        public static OperatorToken Close() =>
            OperatorToken.Close;

        public static WhiteSpaceToken WhiteSpace() =>
            WhiteSpaceToken.Instance;

        public static NumericToken Numeric(string value) =>
            new NumericToken(value);
    }
}
