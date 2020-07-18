using Favalet.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Internal
{
    internal sealed class ReferenceComparer :
        IEqualityComparer<IExpression>,
        IComparer<IExpression>
    {
        public bool Equals(IExpression x, IExpression y) =>
            object.ReferenceEquals(x, y);

        public int GetHashCode(IExpression obj) =>
            RuntimeHelpers.GetHashCode(obj);

        public int Compare(IExpression x, IExpression y) =>
            RuntimeHelpers.GetHashCode(x).CompareTo(RuntimeHelpers.GetHashCode(y));

        public static readonly ReferenceComparer Instance =
            new ReferenceComparer();
    }
}
