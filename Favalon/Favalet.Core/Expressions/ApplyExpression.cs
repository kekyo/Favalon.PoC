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
            IExpression function, IExpression argument, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Function = function;
            this.Argument = argument;
        }

        public IExpression HigherOrder { get; }

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

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IApplyExpression rhs && this.Equals(rhs);

        public IExpression Reduce(IReduceContext context)
        {
            var argument = this.Argument.Reduce(context);

            var function = this.Function;
            while (true)
            {
                if (function is ICallableExpression callable)
                {
                    return callable.Call(context, argument);
                }

                var reducedFunction = function.Reduce(context);

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

        public static ApplyExpression Create(
            IExpression function, IExpression argument, IExpression higherOrder) =>
            new ApplyExpression(function, argument, higherOrder);
        public static ApplyExpression Create(
            IExpression function, IExpression argument) =>
            new ApplyExpression(function, argument, UnspecifiedTerm.Instance);
    }
}
