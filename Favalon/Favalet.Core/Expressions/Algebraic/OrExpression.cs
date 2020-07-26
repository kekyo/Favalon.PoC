using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IOrExpression : IBinaryExpression
    {
    }

    public sealed class OrExpression :
        BinaryExpression<IOrExpression>,
        IOrExpression
    {
        private OrExpression(
            IExpression left, IExpression right, IExpression higherOrder) :
            base(left, right, higherOrder)
        { }

        public override IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);
            var left = this.Left.Infer(context);
            var right = this.Right.Infer(context);

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

        public override IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.FixupHigherOrder(this.HigherOrder);
            var left = this.Left.Fixup(context);
            var right = this.Right.Fixup(context);

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

        public override IExpression Reduce(IReduceContext context)
        {
            var left = this.Left.Reduce(context);
            var right = this.Right.Reduce(context);

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

        public static OrExpression Create(
            IExpression left, IExpression right, IExpression higherOrder) =>
            new OrExpression(left, right, higherOrder);
        public static OrExpression Create(
            IExpression left, IExpression right) =>
            new OrExpression(left, right, UnspecifiedTerm.Instance);
    }
}
