using Favalet.Expressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Favalet.Internal
{
    internal sealed class ReferenceComparer :
        IEqualityComparer<IExpression>,
        IComparer<IExpression>
    {
        [DebuggerStepThrough]
        public bool Equals(IExpression x, IExpression y) =>
            object.ReferenceEquals(x, y);

        [DebuggerStepThrough]
        public int GetHashCode(IExpression obj) =>
            RuntimeHelpers.GetHashCode(obj);

        [DebuggerStepThrough]
        public int Compare(IExpression x, IExpression y) =>
            RuntimeHelpers.GetHashCode(x).CompareTo(RuntimeHelpers.GetHashCode(y));

        public static readonly ReferenceComparer Instance =
            new ReferenceComparer();
    }
}
