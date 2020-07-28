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

            var functionHigherOrder = FunctionExpression.Create(
                parameter.HigherOrder,
                result.HigherOrder,
                context.CreatePlaceholder(PlaceholderOrderHints.KindOrAbove));

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

        public override string GetPrettyString(PrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                context.IsSimple ?
                    $"{this.Parameter.GetPrettyString(context)} -> {this.Result.GetPrettyString(context)}" :
                    $"Function {this.Parameter.GetPrettyString(context)} {this.Result.GetPrettyString(context)}");

        [DebuggerStepThrough]
        public static FunctionExpression Create(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            new FunctionExpression(parameter, result, higherOrder);
        [DebuggerStepThrough]
        public static FunctionExpression Create(
            IExpression parameter, IExpression result) =>
            new FunctionExpression(parameter, result, UnspecifiedTerm.Instance);
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
