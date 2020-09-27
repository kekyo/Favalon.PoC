using System;
using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System.Collections;
using System.Collections.Generic;
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
        Expression, IFunctionExpression, IParentExpression
    {
        public readonly IExpression Parameter;
        public readonly IExpression Result;

        [DebuggerStepThrough]
        private FunctionExpression(
            IExpression parameter, IExpression result, IExpression higherOrder)
        {
            this.Parameter = parameter;
            this.Result = result;
            this.HigherOrder = higherOrder;
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IEnumerable<IExpression> IParentExpression.Children
        {
            [DebuggerStepThrough]
            get
            {
                yield return this.Parameter;
                yield return this.Result;
            }
        }

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Result.GetHashCode();

        public bool Equals(IFunctionExpression rhs) =>
            this.Parameter.Equals(rhs.Parameter) &&
            this.Result.Equals(rhs.Result);

        public override bool Equals(IExpression? other) =>
            other is IFunctionExpression rhs && this.Equals(rhs);

        protected override IExpression MakeRewritable(IMakeRewritableContext context) =>
            InternalCreate(
                context.MakeRewritable(this.Parameter),
                context.MakeRewritable(this.Result),
                () => context.MakeRewritableHigherOrder(this.HigherOrder));

        protected override IExpression Infer(IInferContext context)
        {
            var parameter = context.Infer(this.Parameter);
            var result = context.Infer(this.Result);

            // Recursive inferring exit rule.
            if (parameter is FourthTerm || result is FourthTerm ||
                parameter.HigherOrder is DeadEndTerm || result.HigherOrder is DeadEndTerm)
            {
                if (object.ReferenceEquals(this.Parameter, parameter) &&
                    object.ReferenceEquals(this.Result, result) &&
                    this.HigherOrder is DeadEndTerm)
                {
                    return this;
                }
                else
                {
                    return InternalCreate(
                        parameter,
                        result,
                        () => DeadEndTerm.Instance);
                }
            }
            else
            {
                var higherOrder = context.Infer(this.HigherOrder);

                var functionHigherOrder = Create(
                    parameter.HigherOrder, result.HigherOrder);

                context.Unify(functionHigherOrder, higherOrder);

                if (object.ReferenceEquals(this.Parameter, parameter) &&
                    object.ReferenceEquals(this.Result, result) &&
                    object.ReferenceEquals(this.HigherOrder, higherOrder))
                {
                    return this;
                }
                else
                {
                    return InternalCreate(
                        parameter,
                        result,
                        () => higherOrder);
                }
            }
        }

        protected override IExpression Fixup(IFixupContext context)
        {
            var parameter = context.Fixup(this.Parameter);
            var result = context.Fixup(this.Result);
            var higherOrder = context.FixupHigherOrder(this.HigherOrder);

            if (object.ReferenceEquals(this.Parameter, parameter) &&
                object.ReferenceEquals(this.Result, result) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else if (higherOrder is IFunctionExpression functionExpression)
            {
                return Create(parameter, result, functionExpression);
            }
            else
            {
                // TODO: Apply fixed up higher order.
                //return InternalCreate(parameter, result, () => higherOrder);
                return Create(parameter, result);
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
                return InternalCreate(
                    parameter,
                    result,
                    () => this.HigherOrder);
            }
        }

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            new[] { context.GetXml(this.Parameter), context.GetXml(this.Result) };

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                $"{context.GetPrettyString(this.Parameter)} -> {context.GetPrettyString(this.Result)}");

        private static readonly FunctionExpression Fourth =
            new FunctionExpression(
                FourthTerm.Instance,
                FourthTerm.Instance,
                DeadEndTerm.Instance);

        [DebuggerStepThrough]
        private static IExpression InternalCreate(
            IExpression parameter, IExpression result, Func<IExpression> higherOrder)
        {
            switch (parameter, result)
            {
                case (DeadEndTerm _, _):
                case (_, DeadEndTerm _):
                    return DeadEndTerm.Instance;
                case (FourthTerm _, FourthTerm _):
                    return Fourth;
                case (FourthTerm _, _):
                case (_, FourthTerm _):
                    return new FunctionExpression(
                        parameter,
                        result,
                        DeadEndTerm.Instance);
                default:
                    return new FunctionExpression(
                        parameter,
                        result,
                        higherOrder());
            };
        }

        [DebuggerStepThrough]
        public static FunctionExpression Create(
            IExpression parameter, IExpression result, IFunctionExpression higherOrder) =>
            (FunctionExpression)InternalCreate(
                parameter,
                result,
                () => higherOrder);

        [DebuggerStepThrough]
        private static IExpression CreateRecursivity(
            IExpression parameter, IExpression result) =>
            InternalCreate(
                parameter,
                result,
                () => (parameter, result) switch
                {
                    (UnspecifiedTerm _, UnspecifiedTerm _) =>
                        DeadEndTerm.Instance,
                    (UnspecifiedTerm _, _) =>
                        CreateRecursivity(UnspecifiedTerm.Instance, result.HigherOrder),
                    (_, UnspecifiedTerm _) =>
                        CreateRecursivity(parameter.HigherOrder, UnspecifiedTerm.Instance),
                    _ =>
                        CreateRecursivity(parameter.HigherOrder, result.HigherOrder)
                });

        [DebuggerStepThrough]
        public static FunctionExpression Create(
            IExpression parameter, IExpression result) =>
            (FunctionExpression)CreateRecursivity(parameter, result);
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
