using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IOrExpression : IOperandExpression
    {
    }

    public sealed class OrExpression :
        OperandExpression<IOrExpression>,
        IOrExpression
    {
        private OrExpression(IExpression[] operands) :
            base(operands)
        { }

        public static OrExpression Create(IExpression[] operands) =>
            new OrExpression(operands);
    }
}
