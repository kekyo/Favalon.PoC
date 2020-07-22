using System;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface ICallableExpression : IExpression
    {
        IExpression Call(IReduceContext context, IExpression argument);
    }

    public interface ILambdaExpression : ICallableExpression
    {
        string Parameter { get; }

        IExpression Body { get; }
    }

    public sealed class LambdaExpression :
        Expression, ILambdaExpression
    {
        public readonly string Parameter;
        public readonly IExpression Body;

        private LambdaExpression(string parameter, IExpression body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string ILambdaExpression.Parameter =>
            this.Parameter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression ILambdaExpression.Body =>
            this.Body;

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Body.GetHashCode();

        public bool Equals(ILambdaExpression rhs) =>
            this.Parameter.Equals(rhs.Parameter) &&
            this.Body.Equals(rhs.Body);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is ILambdaExpression rhs && Equals(rhs);

        public IExpression Reduce(IReduceContext context)
        {
            var body = this.Body.Reduce(context);

            if (object.ReferenceEquals(this.Body, body))
            {
                return this;
            }
            else
            {
                return new LambdaExpression(this.Parameter, body);
            }
        }

        public IExpression Call(IReduceContext context, IExpression argument)
        {
            var localContext = context.NewScope(this.Parameter, argument);
            var body = this.Body.Reduce(localContext);

            if (object.ReferenceEquals(this.Body, body))
            {
                return this;
            }
            else
            {
                return body;
            }
        }

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple => $"({this.Parameter} -> {this.Body.GetPrettyString(type)})",
                _ => $"(Lambda {this.Parameter} {this.Body.GetPrettyString(type)})"
            };

        public static LambdaExpression Create(string parameter, IExpression body) =>
            new LambdaExpression(parameter, body);
    }
}
