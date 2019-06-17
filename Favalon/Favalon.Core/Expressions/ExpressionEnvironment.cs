using System;
using System.Collections.Generic;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class ExpressionEnvironment
    {
        public ExpressionEnvironment() { }

        internal ExpressionEnvironment NewScope()
        {
            return this;
        }
    }
}
