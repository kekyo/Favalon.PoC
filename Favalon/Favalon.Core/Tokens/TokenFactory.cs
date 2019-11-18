using Favalon.LexRunners;
using System;
using System.Collections.Generic;

namespace Favalon.Tokens
{
    partial class Token
    {
        public static IEnumerable<char> OperatorChars =>
            LexRunner.operatorChars;

        public static IdentityToken Identity(string identity) =>
            new IdentityToken(identity);

        public static NumericalSignToken NumericalSign(char symbol) =>
            LexRunner.IsNumericSign(symbol) ?
                new NumericalSignToken(symbol) :
                throw new InvalidOperationException();

        public static ParenthesesToken Open(char symbol) =>
            LexRunner.IsOpenParenthesis(symbol) is LexRunner.Parenthesis ?
                new ParenthesesToken(symbol) :
                throw new InvalidOperationException();

        public static ParenthesesToken Close(char symbol) =>
            LexRunner.IsCloseParenthesis(symbol) is LexRunner.Parenthesis ?
                new ParenthesesToken(symbol) :
                throw new InvalidOperationException();

        public static WhiteSpaceToken WhiteSpace() =>
            WhiteSpaceToken.Instance;

        public static NumericToken Numeric(string value) =>
            new NumericToken(value);
    }
}
