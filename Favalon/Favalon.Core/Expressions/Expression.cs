﻿using Favalon.Expressions.Internals;
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

        public string ReadableString =>
            FormatReadableString(new FormatContext(false, false, false), this, false);

        public override string ToString() =>
            $"{this.GetType().Name.Replace("Expression", string.Empty)}: {FormatReadableString(new FormatContext(true, true, false), this, false)}";

        protected abstract (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context);

        protected abstract Expression VisitInferring(IInferringEnvironment environment, InferContext context);

        protected abstract (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context);

        /////////////////////////////////////////////////////////////////////////
        
        private static string FormatEnclosingParenthesesIfRequired(FormatContext context, Expression expression, bool encloseParenthesesIfRequired)
        {
            var (formatted, rp) = expression.FormatReadableString(context);
            return (rp && encloseParenthesesIfRequired) ? $"({formatted})" : formatted;
        }

        protected static string FormatReadableString(FormatContext context, Expression expression, bool encloseParenthesesIfRequired) =>
            (context.WithAnnotation && (expression.HigherOrder != null) && !(expression.HigherOrder is UndefinedExpression)) ?
                $"{FormatEnclosingParenthesesIfRequired(context, expression, true)}:{FormatEnclosingParenthesesIfRequired(context.NewDerived(false, null), expression.HigherOrder, true)}" :
                FormatEnclosingParenthesesIfRequired(context, expression, encloseParenthesesIfRequired);

        protected internal static TExpression VisitInferring<TExpression>(
            IInferringEnvironment environment, TExpression expression, InferContext context)
            where TExpression : TermExpression =>
            (TExpression)expression.VisitInferring(environment, context);

        protected internal static TermExpression VisitInferringHigherOrder(
            IInferringEnvironment environment, TermExpression higherOrder, InferContext context)
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

        protected internal static (bool isResolved, TExpression resolved) VisitResolving<TExpression>(
            IResolvingEnvironment environment, TExpression expression, InferContext context)
            where TExpression : Expression
        {
            var (isResolved, resolved) = expression.VisitResolving(environment, context);
            return (isResolved, (TExpression)resolved);
        }

        protected internal static (bool isResolved, TermExpression resolved) VisitResolvingHigherOrder(
            IResolvingEnvironment environment, TermExpression higherOrder, InferContext context)
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

        protected internal static void Unify(IInferringEnvironment environment, TermExpression expression1, TermExpression expression2) =>
            environment.Unify(expression1, expression2);
    }
}
