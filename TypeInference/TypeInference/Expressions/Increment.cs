using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeInferences.Types;

namespace TypeInferences.Expressions
{
    public sealed class Increment : AvalonExpression
    {
        private readonly AvalonExpression parameter;

        internal Increment(AvalonExpression parameter) =>
            this.parameter = parameter;

        public override AvalonType InferenceType =>
            this.parameter.InferenceType.ToWide(AvalonType.FromClrType<int>());
    }
}
