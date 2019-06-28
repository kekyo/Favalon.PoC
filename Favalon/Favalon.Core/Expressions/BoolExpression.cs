using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class BoolExpression : ConstantExpression<bool>
    {
        internal BoolExpression(bool value) :
            base(value)
        {
        }

        public override string ReadableString =>
            this.Value.ToString();
    }
}
