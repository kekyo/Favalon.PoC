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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression IFunctionExpression.Parameter =>
            this.Parameter;

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

        public override IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);
            var parameter = this.Parameter.Infer(context);
            var result = this.Result.Infer(context);

            var functionHigherOrder = FunctionExpression.Create(
                parameter.HigherOrder,
                result.HigherOrder);

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

        public override IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.FixupHigherOrder(this.HigherOrder);
            var parameter = this.Parameter.Fixup(context);
            var result = this.Result.Fixup(context);

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

        public override IExpression Reduce(IReduceContext context)
        {
            var parameter = this.Parameter.Reduce(context);
            var result = this.Result.Reduce(context);

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
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    $"{this.Parameter.GetPrettyString(context)} -> {this.Result.GetPrettyString(context)}" :
                    $"Function {this.Parameter.GetPrettyString(context)} {this.Result.GetPrettyString(context)}");

        public static FunctionExpression Create(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            new FunctionExpression(parameter, result, higherOrder);
        public static FunctionExpression Create(
            IExpression parameter, IExpression result) =>
            new FunctionExpression(parameter, result, UnspecifiedTerm.Instance);
    }
}
