using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeInferences.Types;

namespace TypeInferences.Expressions
{
    public sealed class Increment : Apply
    {
        internal Increment(AvalonExpression parameter)
            : base(parameter)
        {
        }

        public override AvalonType InferenceType =>
            AvalonType.Wide(base.parameter.InferenceType, Int32Type.Instance);
    }
}
