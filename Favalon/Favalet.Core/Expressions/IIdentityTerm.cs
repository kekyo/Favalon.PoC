using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface IIdentityTerm : ITerm
    {
        string Symbol { get; }
    }

    public static class IdentityTermExtension
    {
        [DebuggerStepThrough]
        public static void Deconstruct(
            this IIdentityTerm identity,
            out string symbol) =>
            symbol = identity.Symbol;
    }
}
