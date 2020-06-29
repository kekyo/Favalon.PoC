using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet
{
    public sealed class ExpressionExactComparer : IEqualityComparer<IExpression>
    {
        private ExpressionExactComparer()
        { }

        public bool Equals(IExpression x, IExpression y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            return (x, y) switch
            {
                (IIdentityTerm idx, IIdentityTerm idy) => idx.Symbol.Equals(idy.Symbol),
                _ => false
            };
        }

        public int GetHashCode(IExpression obj) =>
            obj.GetHashCode();

        public static readonly ExpressionExactComparer Instance =
            new ExpressionExactComparer();
    }
}
