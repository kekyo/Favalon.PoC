namespace Favalon.Tokens
{
    public sealed class OpenParenthesisToken :
        SymbolToken
    {
        internal OpenParenthesisToken(char symbol) :
            base(symbol)
        { }
    }

    public sealed class CloseParenthesisToken :
        SymbolToken
    {
        internal CloseParenthesisToken(char symbol) :
            base(symbol)
        { }
    }
}
