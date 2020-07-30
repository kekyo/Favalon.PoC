using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
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

        private FunctionExpression(
            IExpression parameter, IExpression result, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Parameter = parameter;
            this.Result = result;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IFunctionExpression.Parameter =>
            this.Parameter;

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IFunctionExpression.Result =>
            this.Result;

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

            var functionHigherOrder = From(
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

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                $"{context.GetPrettyString(this.Parameter)} -> {context.GetPrettyString(this.Result)}");

        [DebuggerStepThrough]
        private static IExpression From(
            IExpression parameter, IExpression result, Func<IExpression> higherOrder) =>
            (parameter, result) switch
            {
                (FourthTerm _, _) => FourthTerm.Instance,
                (_, FourthTerm _) => FourthTerm.Instance,
                _ => new FunctionExpression(parameter, result, higherOrder())
            };

        [DebuggerStepThrough]
        public static IExpression From(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            From(parameter, result, () => higherOrder);
        [DebuggerStepThrough]
        public static IExpression From(
            IExpression parameter, IExpression result) =>
            From(parameter, result, () => UnspecifiedTerm.Instance);

        [DebuggerStepThrough]
        internal static IExpression From(
            IExpression parameter, IExpression result, IReduceContext context, PlaceholderOrderHints orderHint) =>
            From(parameter, result, () => context.CreatePlaceholder(orderHint));
    }

    public static class FunctionExpressionExtension
    {
        [DebuggerHidden]
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
