using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract class TermExpression : Expression
    {
        protected TermExpression(TermExpression higherOrder) :
            base(higherOrder)
        {
        }
    }
}
