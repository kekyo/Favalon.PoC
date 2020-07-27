using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System.Diagnostics;

namespace Favalet.Expressions.Algebraic
{
    public interface IAndExpression : IBinaryExpression
    {
    }

    public sealed class AndExpression :
        BinaryExpression<IAndExpression>,
        IAndExpression
    {
        [DebuggerStepThrough]
        private AndExpression(
            IExpression left, IExpression right, IExpression higherOrder) :
            base(left, right, higherOrder)
        { }

        protected override IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);
            var left = context.Infer(this.Left);
            var right = context.Infer(this.Right);

            var andHigherOrder = new AndExpression(
                left.HigherOrder,
                right.HigherOrder,
                UnspecifiedTerm.Instance);

            context.Unify(andHigherOrder, higherOrder);

            if (object.ReferenceEquals(this.Left, left) &&
                object.ReferenceEquals(this.Right, right))
            {
                return this;
            }
            else
            {
                return new AndExpression(left, right, higherOrder);
            }
        }

        protected override IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.Fixup(this.HigherOrder);
            var left = context.Fixup(this.Left);
            var right = context.Fixup(this.Right);

            if (object.ReferenceEquals(this.Left, left) &&
                object.ReferenceEquals(this.Right, right))
            {
                return this;
            }
            else
            {
                return new AndExpression(left, right, higherOrder);
            }
        }

        protected override IExpression Reduce(IReduceContext context)
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
                return new AndExpression(left, right, this.HigherOrder);
            }
        }

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    $"{this.Left.GetPrettyString(context)} && {this.Right.GetPrettyString(context)}" :
                    $"And {this.Left.GetPrettyString(context)} {this.Right.GetPrettyString(context)}");

        [DebuggerStepThrough]
        public static AndExpression Create(
            IExpression left, IExpression right, IExpression higherOrder) =>
            new AndExpression(left, right, higherOrder);
        [DebuggerStepThrough]
        public static AndExpression Create(
            IExpression left, IExpression right) =>
            new AndExpression(left, right, UnspecifiedTerm.Instance);
    }
}
