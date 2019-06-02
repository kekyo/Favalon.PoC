using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TypeInferences.Types;

namespace TypeInferences.Expressions
{
    public class Constant : AvalonExpression
    {
        private readonly object value;

        internal Constant(object value) =>
            this.value = value;

        public override AvalonType InferenceType => AvalonType.FromClrType(this.value.GetType());
    }
}
