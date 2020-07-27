using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface ICallableExpression : IExpression
    {
        IExpression Call(IReduceContext context, IExpression argument);
    }

    public interface IApplyExpression : IExpression
    {
        IExpression Function { get; }

        IExpression Argument { get; }
    }

    public sealed class ApplyExpression :
        Expression, IApplyExpression
    {
        public readonly IExpression Function;
        public readonly IExpression Argument;

        private ApplyExpression(
            IExpression function,
            IExpression argument,
            IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Function = function;
            this.Argument = argument;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IApplyExpression.Function =>
            this.Function;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IApplyExpression.Argument =>
            this.Argument;

        public override int GetHashCode() =>
            this.Function.GetHashCode() ^ this.Argument.GetHashCode();

        public bool Equals(IApplyExpression rhs) =>
            this.Function.Equals(rhs.Function) &&
            this.Argument.Equals(rhs.Argument);

        public override bool Equals(IExpression? other) =>
            other is IApplyExpression rhs && this.Equals(rhs);

        protected override IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);
            var argument = context.Infer(this.Argument);
            var function = context.Infer(this.Function);

            var functionHigherOrder = FunctionExpression.Create(
                argument.HigherOrder,
                higherOrder);

            context.Unify(function.HigherOrder, functionHigherOrder);

            if (object.ReferenceEquals(this.Argument, argument) &&
                object.ReferenceEquals(this.Function, function) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new ApplyExpression(function, argument, higherOrder);
            }
        }

        protected override IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.Fixup(this.HigherOrder);
            var argument = context.Fixup(this.Argument);
            var function = context.Fixup(this.Function);

            if (object.ReferenceEquals(this.Argument, argument) &&
                object.ReferenceEquals(this.Function, function) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new ApplyExpression(function, argument, higherOrder);
            }
        }

        protected override IExpression Reduce(IReduceContext context)
        {
            var argument = context.Reduce(this.Argument);

            var function = this.Function;
            while (true)
            {
                if (function is ICallableExpression callable)
                {
                    return callable.Call(context, argument);
                }

                var reducedFunction = context.Reduce(function);

                if (object.ReferenceEquals(this.Function, reducedFunction) &&
                    object.ReferenceEquals(this.Argument, argument))
                {
                    return this;
                }

                if (object.ReferenceEquals(function, reducedFunction))
                {
                    return new ApplyExpression(reducedFunction, argument, this.HigherOrder);
                }

                function = reducedFunction;
            }
        }

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    $"{this.Function.GetPrettyString(context)} {this.Argument.GetPrettyString(context)}" :
                    $"Apply {this.Function.GetPrettyString(context)} {this.Argument.GetPrettyString(context)}");

        [DebuggerStepThrough]
        public static ApplyExpression Create(
            IExpression function, IExpression argument, IExpression higherOrder) =>
            new ApplyExpression(function, argument, higherOrder);
        [DebuggerStepThrough]
        public static ApplyExpression Create(
            IExpression function, IExpression argument) =>
            new ApplyExpression(function, argument, UnspecifiedTerm.Instance);
    }
}
