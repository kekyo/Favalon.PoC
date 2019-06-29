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

            [DebuggerNonUserCode]
            internal PartialVariable(string name, TermExpression higherOrder)
            {
                this.name = name;
                this.higherOrder = higherOrder;
            }

            [DebuggerNonUserCode]
            public static implicit operator TermExpression(PartialVariable pv) =>
                new FreeVariableExpression(pv.name, pv.higherOrder);

            [DebuggerNonUserCode]
            public static implicit operator VariableExpression(PartialVariable pv) =>
                new FreeVariableExpression(pv.name, pv.higherOrder);

            [DebuggerNonUserCode]
            public static implicit operator FreeVariableExpression(PartialVariable pv) =>
                new FreeVariableExpression(pv.name, pv.higherOrder);

            [DebuggerNonUserCode]
            public static implicit operator BoundVariableExpression(PartialVariable pv) =>
                new BoundVariableExpression(pv.name, pv.higherOrder);

            public override string ToString() =>
                $"PartialVariable: {this.name}:{this.higherOrder.ReadableString}";
        }

        [DebuggerNonUserCode]
        public static PartialVariable Variable(string name) =>
            new PartialVariable(name, UndefinedExpression.Instance);
        [DebuggerNonUserCode]
        public static PartialVariable Variable(string name, TermExpression higherOrder) =>
            new PartialVariable(name, higherOrder);

        [DebuggerNonUserCode]
        public static PlaceholderExpression Placeholder(this Environment environment) =>
            environment.CreatePlaceholder(UndefinedExpression.Instance);
        [DebuggerNonUserCode]
        public static PlaceholderExpression Placeholder(this Environment environment, TermExpression higherOrder) =>
            environment.CreatePlaceholder(higherOrder);

        [DebuggerNonUserCode]
        public static ApplyExpression Apply(TermExpression function, TermExpression parameter) =>
            new ApplyExpression(function, parameter,
                (function is LambdaExpression lambda) ? lambda.Expression.HigherOrder : UndefinedExpression.Instance);

        [DebuggerNonUserCode]
        public static LambdaExpression Lambda(TermExpression parameter, TermExpression expression) =>
            new LambdaExpression(parameter, expression, new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, KindExpression.Instance));

        [DebuggerNonUserCode]
        public static ConstantExpression Constant(object value) =>
            ConstantExpression.Create(value);

        [DebuggerNonUserCode]
        public static KindExpression Kind() =>
            KindExpression.Instance;

        [DebuggerNonUserCode]
        public static TypeExpression Type(string typeName) =>
            new TypeExpression(typeName);

        [DebuggerNonUserCode]
        public static BindExpression Bind(BoundVariableExpression bound, TermExpression expression, TermExpression body) =>
            new BindExpression(bound, expression, body, body.HigherOrder);
    }
}
