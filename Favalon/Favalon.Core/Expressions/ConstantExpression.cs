using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract class ConstantExpression : ValueExpression
    {
        [DebuggerStepperBoundary]
        internal ConstantExpression(TermExpression higherOrder) :
            base(higherOrder)
        { }

        public object Value =>
            this.GetValue();

        internal abstract object GetValue();

        protected override sealed Expression VisitInferring(Environment environment, InferContext context) =>
            this;

        protected override (bool isResolved, Expression resolved) VisitResolving(Environment environment, InferContext context) =>
            (false, this);

        internal static ConstantExpression Create(object value) =>
            value switch
            {
                bool boolValue => (ConstantExpression)new BoolExpression(boolValue),
                string stringValue => new StringExpression(stringValue),
                Unit _ => UnitExpression.Instance,
                _ => new LiteralExpression(value)
            };
    }

    public abstract class ConstantExpression<T> : ConstantExpression
    {
        private static readonly TypeExpression higherOrder =
            new TypeExpression(typeof(T).FullName);

        public new readonly T Value;

        internal ConstantExpression(T value) :
            base(higherOrder) =>
            this.Value = value;

        [DebuggerStepperBoundary]
        internal override sealed object GetValue() =>
            this.Value!;
    }
}
