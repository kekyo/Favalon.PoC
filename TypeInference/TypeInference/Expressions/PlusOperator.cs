using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeInference.Types;

namespace TypeInference.Expressions
{
    public sealed class PlusOperator
    {
        private readonly Constant expression1;
        private readonly Constant expression2;

        public PlusOperator(Constant expression1, Constant expression2)
        {
            this.expression1 = expression1;
            this.expression2 = expression2;
        }


        public IEnumerable<AvalonType> CalculatedTypes =>
            expression1.CalculatedTypes.
            Concat(expression2.CalculatedTypes).
            Distinct();
    }
}
