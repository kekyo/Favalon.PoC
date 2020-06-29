using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IAndExpression : IOperandExpression
    {
    }

    public sealed class AndExpression :
        OperandExpression<IAndExpression>,
        IAndExpression
    {
        private AndExpression(IExpression[] operands) :
            base(operands)
        { }

        public static AndExpression Create(IExpression[] operands) =>
            new AndExpression(operands);
    }
}
