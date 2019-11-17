using Favalon.LexRunners;
using System.Collections.Generic;

namespace Favalon.Tokens
{
    partial class Token
    {
        internal static readonly IdentityToken open =
            new IdentityToken("(");
        internal static readonly IdentityToken close =
            new IdentityToken(")");

        public static IEnumerable<char> OperatorChars =>
            Runner.operatorChars;

        public static IdentityToken Identity(string identity) =>
            new IdentityToken(identity);

        public static NumericalSignToken NumericalSign(char symbol) =>
            new NumericalSignToken(symbol);

        public static IdentityToken Open() =>
            open;

        public static IdentityToken Close() =>
            close;

        public static WhiteSpaceToken WhiteSpace() =>
            WhiteSpaceToken.Instance;

        public static NumericToken Numeric(string value) =>
            new NumericToken(value);
    }
}
