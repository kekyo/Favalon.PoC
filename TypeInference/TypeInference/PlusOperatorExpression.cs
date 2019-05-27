using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypeInference
{
    public sealed class PlusOperatorExpression
    {
        private readonly ConstantExpression expression1;
        private readonly ConstantExpression expression2;

        public PlusOperatorExpression(ConstantExpression expression1, ConstantExpression expression2)
        {
            this.expression1 = expression1;
            this.expression2 = expression2;
        }


        public IEnumerable<Type> CalculatedTypes =>
            expression1.CalculatedTypes.
            Concat(expression2.CalculatedTypes).
            Distinct();
    }
}
