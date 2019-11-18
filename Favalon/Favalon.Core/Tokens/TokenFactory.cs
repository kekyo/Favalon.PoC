using Favalon.Internal;
using Favalon.LexRunners;
using System;
using System.Collections.Generic;

namespace Favalon.Tokens
{
    partial class Token
    {
        public static IEnumerable<char> OperatorChars =>
            Characters.operatorChars;

        public static IdentityToken Identity(string identity) =>
            new IdentityToken(identity);

        public static NumericalSignToken NumericalSign(char symbol) =>
            Characters.IsNumericSign(symbol) ?
                new NumericalSignToken(symbol) :
                throw new InvalidOperationException();

        public static OpenParenthesisToken Open(char symbol) =>
            Characters.IsOpenParenthesis(symbol) is ParenthesisInformation ?
                new OpenParenthesisToken(symbol) :
                throw new InvalidOperationException();

        public static CloseParenthesisToken Close(char symbol) =>
            Characters.IsCloseParenthesis(symbol) is ParenthesisInformation ?
                new CloseParenthesisToken(symbol) :
                throw new InvalidOperationException();

        public static WhiteSpaceToken WhiteSpace() =>
            WhiteSpaceToken.Instance;

        public static NumericToken Numeric(string value) =>
            new NumericToken(value);
    }
}
