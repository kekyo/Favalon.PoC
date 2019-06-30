using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalon.Expressions.Internals
{
    public static class StaticFactory
    {
        public struct PartialVariable
        {
            private readonly string name;
            private readonly TermExpression higherOrder;

            [DebuggerStepThrough]
            internal PartialVariable(string name, TermExpression higherOrder)
            {
                this.name = name;
                this.higherOrder = higherOrder;
            }

            [DebuggerStepThrough]
            public static implicit operator TermExpression(PartialVariable pv) =>
                new FreeVariableExpression(pv.name, pv.higherOrder);

            [DebuggerStepThrough]
            public static implicit operator VariableExpression(PartialVariable pv) =>
                new FreeVariableExpression(pv.name, pv.higherOrder);

            [DebuggerStepThrough]
            public static implicit operator FreeVariableExpression(PartialVariable pv) =>
                new FreeVariableExpression(pv.name, pv.higherOrder);

            [DebuggerStepThrough]
            public static implicit operator BoundVariableExpression(PartialVariable pv) =>
                new BoundVariableExpression(pv.name, pv.higherOrder);

            public override string ToString() =>
                $"PartialVariable: {this.name}:{this.higherOrder.ReadableString}";
        }

        [DebuggerStepThrough]
        public static PartialVariable Variable(string name) =>
            new PartialVariable(name, UndefinedExpression.Instance);
        [DebuggerStepThrough]
        public static PartialVariable Variable(string name, TermExpression higherOrder) =>
            new PartialVariable(name, higherOrder);

        [DebuggerStepThrough]
        public static PlaceholderExpression Placeholder(this Environment environment) =>
            environment.CreatePlaceholder(UndefinedExpression.Instance);
        [DebuggerStepThrough]
        public static PlaceholderExpression Placeholder(this Environment environment, TermExpression higherOrder) =>
            environment.CreatePlaceholder(higherOrder);

        [DebuggerStepThrough]
        public static ApplyExpression Apply(TermExpression function, TermExpression parameter) =>
            new ApplyExpression(function, parameter,
                (function is LambdaExpression lambda) ? lambda.Expression.HigherOrder : UndefinedExpression.Instance);

        [DebuggerStepThrough]
        public static LambdaExpression Lambda(TermExpression parameter, TermExpression expression) =>
            new LambdaExpression(parameter, expression, new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, KindExpression.Instance));

        [DebuggerStepThrough]
        public static ConstantExpression Constant(object value) =>
            ConstantExpression.Create(value);

        [DebuggerStepThrough]
        public static KindExpression Kind() =>
            KindExpression.Instance;

        [DebuggerStepThrough]
        public static TypeExpression Type(string typeName) =>
            new TypeExpression(typeName);

        [DebuggerStepThrough]
        public static BindExpression Bind(BoundVariableExpression bound, TermExpression expression, TermExpression body) =>
            new BindExpression(bound, expression, body, body.HigherOrder);
    }
}
