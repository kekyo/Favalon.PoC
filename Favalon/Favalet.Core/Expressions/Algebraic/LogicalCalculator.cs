using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    public interface ILogicalCalculator
    {
        bool Equals(IExpression lhs, IExpression rhs);

        IExpression Compute(IExpression operand);
    }

    public class LogicalCalculator :
        ILogicalCalculator
    {
        public bool Equals(IExpression lhs, IExpression rhs) =>
            lhs.Equals(rhs);

        public IExpression Compute(IExpression operand)
        {
            return operand;
        }
    }
}
