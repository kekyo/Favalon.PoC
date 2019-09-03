using System;

namespace Favalon.Tokens
{
    public interface IVariableToken
    {
        string Value { get; }
    }

    public sealed class VariableToken :
        Token<VariableToken>, IVariableToken
    {
        public VariableToken(string value) :
            base(value)
        { }

        string IVariableToken.Value => base.Value;
    }
}
