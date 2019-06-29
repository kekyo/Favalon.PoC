using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract class ValueExpression : TermExpression
    {
        protected ValueExpression(TermExpression higherOrder) :
            base(higherOrder)
        { }
    }
}
