using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeInferences.Types;

namespace TypeInferences.Expressions
{
    public abstract class Apply : AvalonExpression
    {
        internal readonly AvalonExpression parameter;

        internal Apply(AvalonExpression parameter)
        {
            this.parameter = parameter;
        }
    }
}
