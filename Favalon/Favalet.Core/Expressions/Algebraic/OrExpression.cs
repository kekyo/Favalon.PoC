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

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple =>
                    $"({this.Left.GetPrettyString(type)} || {this.Right.GetPrettyString(type)})",
                _ =>
                    $"(Or {this.Left.GetPrettyString(type)} {this.Right.GetPrettyString(type)})"
            };

        public static OrExpression Create(
            IExpression left, IExpression right, IExpression higherOrder) =>
            new OrExpression(left, right, higherOrder);
        public static OrExpression Create(
            IExpression left, IExpression right) =>
            new OrExpression(left, right, UnspecifiedTerm.Instance);
    }
}
