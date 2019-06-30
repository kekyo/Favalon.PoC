using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract partial class Expression
    {
        public readonly TermExpression HigherOrder;

        [DebuggerStepThrough]
        internal Expression(TermExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
        }

        public abstract string ReadableString { get; }

        public override string ToString() =>
            $"{this.GetType().Name.Replace("Expression", string.Empty)}: {this.ReadableString}:{this.HigherOrder.ReadableString}";

        protected abstract Expression VisitInferring(Environment environment, InferContext context);

        protected abstract (bool isResolved, Expression resolved) VisitResolving(Environment environment, InferContext context);

        /////////////////////////////////////////////////////////////////////////

        protected internal static TExpression VisitInferring<TExpression>(Environment environment, TExpression expression, InferContext context)
            where TExpression : TermExpression =>
            (TExpression)expression.VisitInferring(environment, context);

        protected internal static TermExpression VisitInferringHigherOrder(Environment environment, TermExpression higherOrder, InferContext context)
        {
            context.RaiseRank();
            try
            {
                return VisitInferring(environment, higherOrder, context);
            }
            finally
            {
                context.DropRank();
            }
        }

        protected internal static (bool isResolved, TExpression resolved) VisitResolving<TExpression>(Environment environment, TExpression expression, InferContext context)
            where TExpression : Expression
        {
            var (isResolved, resolved) = expression.VisitResolving(environment, context);
            return (isResolved, (TExpression)resolved);
        }

        protected internal static (bool isResolved, TermExpression resolved) VisitResolvingHigherOrder(Environment environment, TermExpression higherOrder, InferContext context)
        {
            context.RaiseRank();
            try
            {
                return VisitResolving(environment, higherOrder, context);
            }
            finally
            {
                context.DropRank();
            }
        }

        protected internal static void Unify(Environment environment, TermExpression expression1, TermExpression expression2) =>
            environment.Unify(expression1, expression2);
    }
}
