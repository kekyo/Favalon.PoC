using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet
{
    public interface IIdentityTerm : ITerm
    {
    }

    public sealed class IdentityTerm : IIdentityTerm
    {
        public readonly string Symbol;

        private IdentityTerm(string symbol) =>
            this.Symbol = symbol;

        public static IdentityTerm Create(string symbol) =>
            new IdentityTerm(symbol);
    }
}
