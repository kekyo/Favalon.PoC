using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
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
        private readonly LazySlim<IExpression> higherOrder;

        public readonly IExpression Parameter;
        public readonly IExpression Result;

        [DebuggerStepThrough]
        private FunctionExpression(
            IExpression parameter, IExpression result, Func<IExpression> higherOrder)
        {
            this.Parameter = parameter;
            this.Result = result;
            this.higherOrder = LazySlim.Create(higherOrder);
        }

        [DebuggerStepThrough]
        private FunctionExpression(
            IExpression parameter, IExpression result, IExpression higherOrder)
        {
            this.Parameter = parameter;
            this.Result = result;
            this.higherOrder = LazySlim.Create(higherOrder);
        }

        public override IExpression HigherOrder =>
            this.higherOrder.Value;

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

        protected override IExpression MakeRewritable(IReduceContext context) =>
            new FunctionExpression(
                context.MakeRewritable(this.Parameter),
                context.MakeRewritable(this.Result),
                context.MakeRewritableHigherOrder(this.HigherOrder));

        protected override IExpression Infer(IReduceContext context)
        {
            var parameter = context.Infer(this.Parameter);
            var result = context.Infer(this.Result);

            if (parameter is FourthTerm ||
                result is FourthTerm)
            {
                if (object.ReferenceEquals(this.Parameter, parameter) &&
                    object.ReferenceEquals(this.Result, result) &&
                    this.HigherOrder is DeadEndTerm)
                {
                    return this;
                }
                else
                {
                    return new FunctionExpression(parameter, result, DeadEndTerm.Instance);
                }
            }
            else
            {
                var higherOrder = context.Infer(this.HigherOrder);

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
        }

        protected override IExpression Fixup(IReduceContext context)
        {
            var parameter = context.Fixup(this.Parameter);
            var result = context.Fixup(this.Result);
            
            // HACK: Force applying DeadEndTerm if expressions are 4th order.
            var higherOrder =
                (parameter is FourthTerm || result is FourthTerm) ?
                    DeadEndTerm.Instance :
                    context.Fixup(this.HigherOrder);

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
            IExpression parameter,
            IExpression result,
            Func<IExpression> higherOrder) =>
            (parameter, result) switch
            {
                (FourthTerm _, _) => new FunctionExpression(
                    parameter, result, DeadEndTerm.Instance),
                (_, FourthTerm _) => new FunctionExpression(
                    parameter, result, DeadEndTerm.Instance),
                _ => new FunctionExpression(
                    parameter, result, higherOrder)
            };

        [DebuggerStepThrough]
        public static FunctionExpression Create(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            Create(parameter, result, () => higherOrder);
        [DebuggerStepThrough]
        public static FunctionExpression Create(
            IExpression parameter, IExpression result) =>
            Create(parameter, result, () => UnspecifiedTerm.TypeInstance);

        private sealed class LazyHigherOrderPlaceholderFunctionGenerator
        {
            private readonly IReduceContext context;
            private readonly PlaceholderOrderHints orderHint;

            public LazyHigherOrderPlaceholderFunctionGenerator(
                IReduceContext context,
                PlaceholderOrderHints orderHint)
            {
                this.context = context;
                this.orderHint = orderHint;
            }

            public IExpression Create()
            {
                if (this.orderHint >= PlaceholderOrderHints.Fourth)
                {
                    return FunctionExpression.Create(
                        FourthTerm.Instance,
                        FourthTerm.Instance,
                        () => DeadEndTerm.Instance);
                }
                else
                {
                    var generator = new LazyHigherOrderPlaceholderFunctionGenerator(this.context, this.orderHint + 1);
                    return FunctionExpression.Create(
                        context.CreatePlaceholder(this.orderHint),
                        context.CreatePlaceholder(this.orderHint),
                        generator.Create);
                }
            }
        }

        [DebuggerStepThrough]
        internal static FunctionExpression Create(
            IExpression parameter,
            IExpression result,
            IReduceContext context,
            PlaceholderOrderHints orderHint)
        {
            var generator = new LazyHigherOrderPlaceholderFunctionGenerator(context, orderHint);
            return Create(parameter, result, generator.Create);
        }
    }

    [DebuggerStepThrough]
    public static class FunctionExpressionExtension
    {
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
