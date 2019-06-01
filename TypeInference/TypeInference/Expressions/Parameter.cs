using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TypeInferences.Types;

namespace TypeInferences.Expressions
{
    public sealed class Parameter : AvalonExpression
    {
        private readonly string name;
        private readonly AvalonType type;

        internal Parameter(string name, AvalonType type)
        {
            this.name = name;
            this.type = type;
        }

        public override AvalonType InferenceType => this.type;
    }
}
