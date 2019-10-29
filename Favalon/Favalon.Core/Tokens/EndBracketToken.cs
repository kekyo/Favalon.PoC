using System;

namespace Favalon.Tokens
{
    public sealed class EndBracketToken :
        Token, IEquatable<EndBracketToken?>
    {
        private EndBracketToken()
        { }

        public override int GetHashCode() =>
            0;

        public bool Equals(EndBracketToken? other) =>
            other != null;

        public override bool Equals(object obj) =>
            this.Equals(obj as EndBracketToken);

        public override string ToString() =>
            ")";

        internal static readonly EndBracketToken Instance =
            new EndBracketToken();
    }
}
