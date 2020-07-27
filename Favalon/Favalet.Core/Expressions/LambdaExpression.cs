using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface ILambdaExpression :
        ICallableExpression
    {
        IIdentityTerm Parameter { get; }

        IExpression Body { get; }
    }

    public sealed class LambdaExpression :
        Expression, ILambdaExpression
    {
        public readonly IIdentityTerm Parameter;
        public readonly IExpression Body;

        private LambdaExpression(
            IIdentityTerm parameter, IExpression body, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Parameter = parameter;
            this.Body = body;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IIdentityTerm ILambdaExpression.Parameter =>
            this.Parameter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression ILambdaExpression.Body =>
            this.Body;

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Body.GetHashCode();

        public bool Equals(ILambdaExpression rhs) =>
            this.Parameter.Equals(rhs.Parameter) &&
            this.Body.Equals(rhs.Body);

        public override bool Equals(IExpression? other) =>
            other is ILambdaExpression rhs && this.Equals(rhs);

        public override IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);
            var parameter = this.Parameter.Infer(context);
            var body = this.Body.Infer(context);

            var lambdaHigherOrder = FunctionExpression.Create(
                parameter.HigherOrder,
                body.HigherOrder,
                UnspecifiedTerm.Instance);

            context.Unify(lambdaHigherOrder, higherOrder);

            if (object.ReferenceEquals(this.Parameter, parameter) &&
                object.ReferenceEquals(this.Body, body) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else if (parameter is IIdentityTerm identity)
            {
                return new LambdaExpression(identity, body, higherOrder);
            }
            else
            {
                return new LambdaExpression(this.Parameter, body, higherOrder);
            }
        }

        public override IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.FixupHigherOrder(this.HigherOrder);
            var parameter = this.Parameter.Fixup(context);
            var body = this.Body.Fixup(context);

            if (object.ReferenceEquals(this.Parameter, parameter) &&
                object.ReferenceEquals(this.Body, body) &&
                object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else if (parameter is IIdentityTerm identity)
            {
                return new LambdaExpression(identity, body, higherOrder);
            }
            else
            {
                return new LambdaExpression(this.Parameter, body, higherOrder);
            }
        }

        public override IExpression Reduce(IReduceContext context)
        {
            var body = this.Body.Reduce(context);

            if (object.ReferenceEquals(this.Body, body))
            {
                return this;
            }
            else
            {
                return new LambdaExpression(this.Parameter, body, this.HigherOrder);
            }
        }

        public IExpression Call(IReduceContext context, IExpression argument) =>
            this.Body.Reduce(context.NewScope(this.Parameter, argument));

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    $"{this.Parameter} -> {this.Body.GetPrettyString(context)}" :
                    $"Lambda {this.Parameter} {this.Body.GetPrettyString(context)}");

        public static LambdaExpression Create(
            IIdentityTerm parameter, IExpression body, IExpression higherOrder) =>
            new LambdaExpression(parameter, body, higherOrder);
        public static LambdaExpression Create(
            IIdentityTerm parameter, IExpression body) =>
            new LambdaExpression(parameter, body, UnspecifiedTerm.Instance);
    }
}
