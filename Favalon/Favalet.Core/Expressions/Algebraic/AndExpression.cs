using Favalet.Contexts;
using Favalet.Expressions.Specialized;

namespace Favalet.Expressions.Algebraic
{
    public interface IAndExpression : IBinaryExpression
    {
    }

    public sealed class AndExpression :
        BinaryExpression<IAndExpression>,
        IAndExpression
    {
        private AndExpression(
            IExpression left, IExpression right, IExpression higherOrder) :
            base(left, right, higherOrder)
        { }

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
                return new AndExpression(left, right, this.HigherOrder);
            }
        }

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple =>
                    $"({this.Left.GetPrettyString(type)} && {this.Right.GetPrettyString(type)})",
                _ =>
                    $"(And {this.Left.GetPrettyString(type)} {this.Right.GetPrettyString(type)})"
            };

        public static AndExpression Create(
            IExpression left, IExpression right, IExpression higherOrder) =>
            new AndExpression(left, right, higherOrder);
        public static AndExpression Create(
            IExpression left, IExpression right) =>
            new AndExpression(left, right, UnspecifiedTerm.Instance);
    }
}
