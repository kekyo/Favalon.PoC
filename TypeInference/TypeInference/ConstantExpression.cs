using System;
using System.Collections.Generic;
using System.Text;

namespace TypeInference
{
    public sealed class ConstantExpression
    {
        private readonly object value;

        public ConstantExpression(object value) => this.value = value;


        public IEnumerable<Type> CalculatedTypes => new[] { this.value.GetType() };
    }
}
