using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
using System.Collections;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface IFunctionExpression :
        IExpression
    {
        IExpression Parameter { get; }

        IExpression Result { get; }
    }

    public sealed class FunctionExpression :
        Expression, IFunctionExpression
    {
        public readonly IExpression Parameter;
        public readonly IExpression Result;

        [DebuggerStepThrough]
        private FunctionExpression(
            IExpression parameter, IExpression result, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Parameter = parameter;
            this.Result = result;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IFunctionExpression.Parameter
        {
            [DebuggerStepThrough]
            get => this.Parameter;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IFunctionExpression.Result
        {
            [DebuggerStepThrough]
            get => this.Result;
        }

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Result.GetHashCode();

        public bool Equals(IFunctionExpression rhs) =>
            this.Parameter.Equals(rhs.Parameter) &&
            this.Result.Equals(rhs.Result);

        public override bool Equals(IExpression? other) =>
            other is IFunctionExpression rhs && this.Equals(rhs);

        protected override IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);
            var parameter = context.Infer(this.Parameter);
            var result = context.Infer(this.Result);

            var functionHigherOrder = Create(
                parameter.HigherOrder,
                result.HigherOrder,
                context,
                PlaceholderOrderHints.KindOrAbove);

            context.Unify(functionHigherOrder, higherOrder);

            if (object.ReferenceEquals(this.Parameter, parameter) &&
                object.ReferenceEquals(this.Result, result) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new FunctionExpression(parameter, result, higherOrder);
            }
        }

        protected override IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.Fixup(this.HigherOrder);
            var parameter = context.Fixup(this.Parameter);
            var result = context.Fixup(this.Result);

            if (object.ReferenceEquals(this.Parameter, parameter) &&
                object.ReferenceEquals(this.Result, result) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new FunctionExpression(parameter, result, higherOrder);
            }
        }

        protected override IExpression Reduce(IReduceContext context)
        {
            var parameter = context.Reduce(this.Parameter);
            var result = context.Reduce(this.Result);

            if (object.ReferenceEquals(this.Parameter, parameter) &&
                object.ReferenceEquals(this.Result, result))
            {
                return this;
            }
            else
            {
                return new FunctionExpression(parameter, result, this.HigherOrder);
            }
        }

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            new[] { context.GetXml(this.Parameter), context.GetXml(this.Result) };

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                $"{context.GetPrettyString(this.Parameter)} -> {context.GetPrettyString(this.Result)}");

        [DebuggerStepThrough]
        private static FunctionExpression Create(
            IExpression parameter, IExpression result, Func<IExpression> higherOrder) =>
            (parameter, result) switch
            {
                (FourthTerm _, _) => new FunctionExpression(parameter, result, TerminationTerm.Instance),
                (_, FourthTerm _) => new FunctionExpression(parameter, result, TerminationTerm.Instance),
                _ => new FunctionExpression(parameter, result, higherOrder())
            };

        [DebuggerStepThrough]
        public static FunctionExpression Create(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            Create(parameter, result, () => higherOrder);
        [DebuggerStepThrough]
        public static FunctionExpression Create(
            IExpression parameter, IExpression result) =>
            Create(parameter, result, () => UnspecifiedTerm.Instance);

        [DebuggerStepThrough]
        internal static FunctionExpression Create(
            IExpression parameter, IExpression result, IReduceContext context, PlaceholderOrderHints orderHint) =>
            Create(parameter, result, () => context.CreatePlaceholder(orderHint));
    }

    public static class FunctionExpressionExtension
    {
        [DebuggerStepThrough]
        public static void Deconstruct(
            this IFunctionExpression function,
            out IExpression parameter,
            out IExpression result)
        {
            parameter = function.Parameter;
            result = function.Result;
        }
    }
}
