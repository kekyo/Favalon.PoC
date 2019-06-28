using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract class Expression
    {
        public readonly TermExpression HigherOrder;

        internal Expression(TermExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
        }

        public abstract string ReadableString { get; }

        protected abstract Expression VisitInferring(Environment environment);

        protected internal static TExpression VisitInferring<TExpression>(Environment environment, TExpression expression)
            where TExpression : Expression =>
            (TExpression)expression.VisitInferring(environment);
    }
}
