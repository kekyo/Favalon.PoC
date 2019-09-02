using System;

namespace Favalon
{
    public enum TokenTypes
    {
        Numeric,
        Operator,
        String,
        Variable
    }

    public struct Token
    {
        public readonly TokenTypes TokenType;
        public readonly string Value;

        public Token(TokenTypes tokenType, string value)
        {
            this.TokenType = tokenType;
            this.Value = value;
        }

        public override string ToString() =>
            $"{this.TokenType}: {this.Value}";
    }

}
