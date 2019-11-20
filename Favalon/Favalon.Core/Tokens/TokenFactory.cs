using Favalon.Internal;
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

        public static NumericalSignToken PlusSign() =>
            NumericalSignToken.Plus;
        public static NumericalSignToken MinusSign() =>
            NumericalSignToken.Minus;

        public static OpenParenthesisToken Open(char symbol) =>
            Characters.IsOpenParenthesis(symbol) is ParenthesisPair parenthesis ?
                new OpenParenthesisToken(parenthesis) :
                throw new InvalidOperationException();

        public static CloseParenthesisToken Close(char symbol) =>
            Characters.IsCloseParenthesis(symbol) is ParenthesisPair parenthesis ?
                new CloseParenthesisToken(parenthesis) :
                throw new InvalidOperationException();

        public static WhiteSpaceToken WhiteSpace() =>
            WhiteSpaceToken.Instance;

        public static NumericToken Numeric(string value) =>
            new NumericToken(value);
    }
}
