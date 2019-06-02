using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeInferences.Types;

namespace TypeInferences.Expressions
{
    public sealed class Apply : AvalonExpression
    {
        private readonly Lambda target;
        private readonly AvalonExpression[] arguments;

        internal Apply(Lambda target, AvalonExpression[] arguments)
        {
            this.target = target;
            this.arguments = arguments;
        }

        public override AvalonType InferenceType =>
            this.parameter.InferenceType.ToWide(this.arguments);
    }
}
