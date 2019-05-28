using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TypeInferences.Types;

namespace TypeInferences.Expressions
{
    public abstract class AvalonExpression
    {
        protected AvalonExpression() { }

        public abstract AvalonType InferenceType { get; }

        public static AvalonExpression Constant(object value) =>
            new Constant(value);
        public static AvalonExpression Increment(AvalonExpression parameter) =>
            new Increment(parameter);
        public static AvalonExpression Lambda(AvalonExpression body, AvalonExpression parameter) =>
            new Lambda(body, parameter);
    }
}
