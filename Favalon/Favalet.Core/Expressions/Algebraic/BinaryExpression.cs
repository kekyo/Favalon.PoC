using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IBinaryExpression : IExpression
    {
        IExpression Left { get; }
        IExpression Right { get; }
    }

    public abstract class BinaryExpression<TBinaryExpression> :
        Expression, IBinaryExpression
        where TBinaryExpression : IBinaryExpression
    {
        public readonly IExpression Left;
        public readonly IExpression Right;

        protected BinaryExpression(IExpression left, IExpression right)
        {
            this.Left = left;
            this.Right = right;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IBinaryExpression.Left =>
            this.Left;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IBinaryExpression.Right =>
            this.Right;

        public override int GetHashCode() =>
            this.Left.GetHashCode() ^ this.Right.GetHashCode();

        public bool Equals(TBinaryExpression rhs) =>
            this.Left.Equals(rhs.Left) && this.Right.Equals(rhs.Right);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is TBinaryExpression rhs && this.Equals(rhs);

        public abstract IExpression Reduce(IReduceContext context);
    }

    public static class BinaryExpressionExtension
    {
        public static void Deconstruct(
            this IBinaryExpression binary,
            out IExpression left,
            out IExpression right)
        {
            left = binary.Left;
            right = binary.Right;
        }
    }
}
