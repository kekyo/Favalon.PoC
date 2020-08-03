using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface IIdentityTerm :
        ITerm
    {
        string Symbol { get; }
    }

    [DebuggerStepThrough]
    public static class IdentityTermExtension
    {
        public static void Deconstruct(
            this IIdentityTerm identity,
            out string symbol) =>
            symbol = identity.Symbol;
    }
}
