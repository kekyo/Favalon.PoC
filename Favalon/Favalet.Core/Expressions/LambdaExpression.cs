﻿using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System.Collections;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface ILambdaExpression :
        ICallableExpression
    {
        IBoundVariableTerm Parameter { get; }

        IExpression Body { get; }
    }

    public sealed class LambdaExpression :
        Expression, ILambdaExpression
    {
        public readonly IBoundVariableTerm Parameter;
        public readonly IExpression Body;

        [DebuggerStepThrough]
        private LambdaExpression(
            IBoundVariableTerm parameter, IExpression body, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Parameter = parameter;
            this.Body = body;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IBoundVariableTerm ILambdaExpression.Parameter
        {
            [DebuggerStepThrough]
            get => this.Parameter;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression ILambdaExpression.Body
        {
            [DebuggerStepThrough]
            get => this.Body;
        }

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Body.GetHashCode();

        public bool Equals(ILambdaExpression rhs) =>
            this.Parameter.Equals(rhs.Parameter) &&
            this.Body.Equals(rhs.Body);

        public override bool Equals(IExpression? other) =>
            other is ILambdaExpression rhs && this.Equals(rhs);

        protected override IExpression Infer(IReduceContext context)
        {
            var parameter = (IBoundVariableTerm)context.Infer(this.Parameter);
            var higherOrder = context.InferHigherOrder(this.HigherOrder);

            var newScope = context.Bind(parameter, parameter);

            var body = newScope.Infer(this.Body);

            var lambdaHigherOrder = FunctionExpression.Create(
                parameter.HigherOrder,
                body.HigherOrder,
                context,
                PlaceholderOrderHints.TypeOrAbove);

            var inferredLambdaHigherOrder =
                context.InferHigherOrder(lambdaHigherOrder);

            context.Unify(inferredLambdaHigherOrder, higherOrder);

            if (object.ReferenceEquals(this.Parameter, parameter) &&
                object.ReferenceEquals(this.Body, body) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new LambdaExpression(parameter, body, higherOrder);
            }
        }

        protected override IExpression Fixup(IReduceContext context)
        {
            var parameter = (IBoundVariableTerm)context.Fixup(this.Parameter);
            var body = context.Fixup(this.Body);
            var higherOrder = context.Fixup(this.HigherOrder);

            if (object.ReferenceEquals(this.Parameter, parameter) &&
                object.ReferenceEquals(this.Body, body) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new LambdaExpression(parameter, body, higherOrder);
            }
        }

        protected override IExpression Reduce(IReduceContext context)
        {
            var parameter = (IBoundVariableTerm)context.Reduce(this.Parameter);
            var body = context.Reduce(this.Body);

            if (object.ReferenceEquals(this.Parameter, parameter) &&
                object.ReferenceEquals(this.Body, body))
            {
                return this;
            }
            else
            {
                return new LambdaExpression(parameter, body, this.HigherOrder);
            }
        }

        public IExpression Call(IReduceContext context, IExpression argument) =>
            context.
                Bind(this.Parameter, argument).
                Reduce(this.Body);

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            new[] { context.GetXml(this.Parameter), context.GetXml(this.Body) };

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                $"{context.GetPrettyString(this.Parameter)} -> {context.GetPrettyString(this.Body)}");

        [DebuggerStepThrough]
        public static LambdaExpression Create(
            IBoundVariableTerm parameter, IExpression body, IExpression higherOrder) =>
            new LambdaExpression(parameter, body, higherOrder);
        [DebuggerStepThrough]
        public static LambdaExpression Create(
            IBoundVariableTerm parameter, IExpression body) =>
            new LambdaExpression(parameter, body, UnspecifiedTerm.TypeInstance);
    }

    [DebuggerStepThrough]
    public static class LambdaExpressionExtension
    {
        public static void Deconstruct(
            this ILambdaExpression lambda,
            out IBoundVariableTerm parameter,
            out IExpression body)
        {
            parameter = lambda.Parameter;
            body = lambda.Body;
        }
    }
}
