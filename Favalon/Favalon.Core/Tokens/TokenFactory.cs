﻿namespace Favalon.Tokens
{
    partial class Token
    {
        public static IdentityToken Identity(string identity) =>
            new IdentityToken(identity);

        public static BeginBracketToken Begin() =>
            BeginBracketToken.Instance;

        public static EndBracketToken End() =>
            EndBracketToken.Instance;

        public static WhiteSpaceToken WhiteSpace() =>
            WhiteSpaceToken.Instance;

        public static NumericToken Numeric(string value) =>
            new NumericToken(value);
    }
}
