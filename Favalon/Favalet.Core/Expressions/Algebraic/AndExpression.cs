using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IAndExpression : IBinaryExpression
    {
    }

    public sealed class AndExpression :
        BinaryExpression<IAndExpression>,
        IAndExpression
    {
        private AndExpression(IExpression left, IExpression right) :
            base(left, right)
        { }

        public override IExpression Reduce(IReduceContext context) =>
            new AndExpression(
                this.Left.Reduce(context),
                this.Right.Reduce(context));

        public static AndExpression Create(IExpression left, IExpression right) =>
            new AndExpression(left, right);
    }
}
