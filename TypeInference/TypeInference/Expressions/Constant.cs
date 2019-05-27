using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TypeInference.Types;

namespace TypeInference.Expressions
{
    public sealed class Constant
    {
        private readonly object value;

        public Constant(object value) => this.value = value;


        public IEnumerable<AvalonType> CalculatedTypes =>
            new[] { AvalonType.Create(this.value.GetType()) };
    }
}
