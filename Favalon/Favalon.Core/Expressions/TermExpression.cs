using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract class TermExpression : Expression
    {
        [DebuggerNonUserCode]
        protected TermExpression(TermExpression higherOrder) :
            base(higherOrder)
        {
        }
    }
}
