﻿using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public sealed class BindExpression : Expression
    {
        internal BindExpression(FreeVariableExpression bound, Expression expression, Expression body, TextRange textRange) :
            base(body.HigherOrder, textRange)
        {
            this.Bound = bound;
            this.Expression = expression;
            this.Body = body;
        }

        public FreeVariableExpression Bound { get; private set; }
        public Expression Expression { get; private set; }
        public Expression Body { get; private set; }

        protected internal override string FormatReadableString(FormatContext context) =>
            $"{this.Bound.GetReadableString(context)} = {this.Expression.GetReadableString(context)} in {this.Body.GetReadableString(context)}";

        internal static (FreeVariableExpression bound, Expression expression) InternalVisit(
            ExpressionEnvironment environment, InferContext context, FreeVariableExpression bound, Expression expression)
        {
            var e = expression.VisitInferring(environment, context);

            // Force replacing with new free variable.
            // Because the bind expression infers excepted from derived environments,
            // but uses variable expression instead simple name string.
            // It requires annotation processing.
            var b = bound.CloneWithPlaceholderIfUndefined(environment);

            context.UnifyExpression(bound.HigherOrder, expression.HigherOrder);

            return (b, e);
        }

        protected internal override Expression VisitInferring(
            ExpressionEnvironment environment, InferContext context)
        {
            // Bind expression scope details:
            // let x = y in z
            //     |   |    |
            //     | outer  |
            //     |        |
            //     +-inner--+

            var scoped = environment.NewScope();
            var (bound, expression) = InternalVisit(scoped, context, this.Bound, this.Expression);

            scoped.Bind(bound.Name, expression);

            var body = this.Body.VisitInferring(scoped, context);

            return new BindExpression(bound, expression, body, this.TextRange);
        }

        protected internal override TraverseInferringResults FixupHigherOrders(InferContext context, int rank)
        {
            this.Bound = context.FixupHigherOrders(this.Bound, rank);
            this.Expression = context.FixupHigherOrders(this.Expression, rank);
            this.Body = context.FixupHigherOrders(this.Body, rank);

            return TraverseInferringResults.RequeireHigherOrder;
        }

        protected internal override IEnumerable<XObject> CreateXmlChildren(FormatContext context) =>
            new[] { this.Bound.CreateXml(context), this.Expression.CreateXml(context), this.Body.CreateXml(context) };
    }
}
