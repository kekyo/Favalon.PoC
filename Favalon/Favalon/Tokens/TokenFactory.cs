namespace Favalon.Tokens
{
    partial class Token
    {
        public static IdentityToken Identity(string identity) =>
            new IdentityToken(identity);
    }
}
