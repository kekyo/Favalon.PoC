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
        private OrExpression(IExpression left, IExpression right) :
            base(left, right)
        { }

        public override IExpression Reduce(IReduceContext context) =>
            new OrExpression(
                this.Left.Reduce(context),
                this.Right.Reduce(context));

        public static OrExpression Create(IExpression left, IExpression right) =>
            new OrExpression(left, right);
    }
}
