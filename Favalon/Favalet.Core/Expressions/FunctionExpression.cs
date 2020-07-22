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

        public IExpression HigherOrder { get; }

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

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IFunctionExpression rhs && this.Equals(rhs);

        public IExpression Reduce(IReduceContext context)
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

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple => $"({this.Parameter.GetPrettyString(type)} -> {this.Result.GetPrettyString(type)})",
                _ => $"(Function {this.Parameter.GetPrettyString(type)} {this.Result.GetPrettyString(type)})"
            };

        public static FunctionExpression Create(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            new FunctionExpression(parameter, result, higherOrder);
        public static FunctionExpression Create(
            IExpression parameter, IExpression result) =>
            new FunctionExpression(parameter, result, UnspecifiedTerm.Instance);
    }
}
