using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System.Diagnostics;

namespace Favalet.Expressions.Algebraic
{
    public interface IOrExpression : IBinaryExpression
    {
    }

    public sealed class OrExpression :
        BinaryExpression<IOrExpression>,
        IOrExpression
    {
        [DebuggerStepThrough]
        private OrExpression(
            IExpression left, IExpression right, IExpression higherOrder) :
            base(left, right, higherOrder)
        { }

        protected override IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);
            var left = context.Infer(this.Left);
            var right = context.Infer(this.Right);

            var andHigherOrder = new OrExpression(
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
                return new OrExpression(left, right, higherOrder);
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
                return new OrExpression(left, right, higherOrder);
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
                return new OrExpression(left, right, this.HigherOrder);
            }
        }

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    $"{this.Left.GetPrettyString(context)} || {this.Right.GetPrettyString(context)}" :
                    $"Or {this.Left.GetPrettyString(context)} {this.Right.GetPrettyString(context)}");

        [DebuggerStepThrough]
        public static OrExpression Create(
            IExpression left, IExpression right, IExpression higherOrder) =>
            new OrExpression(left, right, higherOrder);
        [DebuggerStepThrough]
        public static OrExpression Create(
            IExpression left, IExpression right) =>
            new OrExpression(left, right, UnspecifiedTerm.Instance);
    }
}
