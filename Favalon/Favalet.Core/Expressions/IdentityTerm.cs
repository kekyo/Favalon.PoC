using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions
{
    public interface IIdentityTerm : ITerm
    {
        string Symbol { get; }
    }

    public sealed class IdentityTerm : IIdentityTerm
    {
        public readonly string Symbol;

        private IdentityTerm(string symbol) =>
            this.Symbol = symbol;

        string IIdentityTerm.Symbol =>
            this.Symbol;

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(IIdentityTerm rhs) =>
            this.Symbol.Equals(rhs.Symbol);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IIdentityTerm rhs && Equals(rhs);

        public IExpression Reduce(IReduceContext context)
        {
            return this;
        }

        public static IdentityTerm Create(string symbol) =>
            new IdentityTerm(symbol);
    }
}
