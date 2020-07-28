using Favalet.Contexts;
using System;
using System.Diagnostics;

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

        [DebuggerStepThrough]
        protected BinaryExpression(
            IExpression left, IExpression right, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Left = left;
            this.Right = right;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IBinaryExpression.Left =>
            this.Left;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IBinaryExpression.Right =>
            this.Right;

        internal abstract IExpression OnCreate(
            IExpression left, IExpression right, IExpression higherOrder);

        protected override sealed IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);
            var left = context.Infer(this.Left);
            var right = context.Infer(this.Right);

            context.Unify(left.HigherOrder, higherOrder);
            context.Unify(right.HigherOrder, higherOrder);

            if (object.ReferenceEquals(this.Left, left) &&
                object.ReferenceEquals(this.Right, right) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return this.OnCreate(left, right, higherOrder);
            }
        }

        protected override sealed IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.Fixup(this.HigherOrder);
            var left = context.Fixup(this.Left);
            var right = context.Fixup(this.Right);

            if (object.ReferenceEquals(this.Left, left) &&
                object.ReferenceEquals(this.Right, right) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return this.OnCreate(left, right, higherOrder);
            }
        }

        protected override sealed IExpression Reduce(IReduceContext context)
        {
            var left = context.Reduce(this.Left);
            var right = context.Reduce(this.Right);

            if (object.ReferenceEquals(this.Left, left) &&
                object.ReferenceEquals(this.Right, right))
            {
                return this;
            }
            else
            {
                return this.OnCreate(left, right, this.HigherOrder);
            }
        }

        public override int GetHashCode() =>
            this.Left.GetHashCode() ^ this.Right.GetHashCode();

        public bool Equals(TBinaryExpression rhs) =>
            this.Left.Equals(rhs.Left) && this.Right.Equals(rhs.Right);

        public override bool Equals(IExpression? other) =>
            other is TBinaryExpression rhs && this.Equals(rhs);
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
