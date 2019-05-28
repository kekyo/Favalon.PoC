using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeInferences.Types;

namespace TypeInferences.Expressions
{
    public sealed class Lambda : AvalonExpression
    {
        private readonly AvalonExpression body;

        internal Lambda(AvalonExpression body, AvalonExpression parameter)
        {
            this.body = body;
            this.Parameter = parameter;
        }

        public AvalonExpression Parameter { get; }

        public override AvalonType InferenceType =>
            body.InferenceType;
    }
}
