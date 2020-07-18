using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IAndBinaryExpression : IBinaryExpression
    {
    }

    public sealed class AndBinaryExpression :
        BinaryExpression<IAndBinaryExpression>,
        IAndBinaryExpression
    {
        private AndBinaryExpression(IExpression left, IExpression right) :
            base(left, right)
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
                return new AndBinaryExpression(left, right);
            }
        }

        public static AndBinaryExpression Create(IExpression left, IExpression right) =>
            new AndBinaryExpression(left, right);
    }
}
