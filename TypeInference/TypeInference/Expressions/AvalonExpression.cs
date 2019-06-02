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

        public static Constant Constant(object value) =>
            new Constant(value);
        public static Increment Increment(AvalonExpression parameter) =>
            new Increment(parameter);

        public static Parameter Parameter(string name) =>
            new Parameter(name, AvalonType.Unspecified);
        public static Parameter Parameter(string name, AvalonType type) =>
            new Parameter(name, type);
        public static Lambda Lambda(AvalonExpression body, AvalonExpression parameter) =>
            new Lambda(body, parameter);
        public static Apply Apply(Lambda target, params AvalonExpression[] arguments) =>
            new Apply(target, arguments);
    }
}
