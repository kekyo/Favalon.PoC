using System;
using System.Reflection;

namespace Favalon.Expressions
{
    public sealed class Lambda : Expression
    {
        public readonly Expression Parameter;
        public readonly Expression Body;

        internal Lambda(Expression parameter, Expression body)
        {
            this.Parameter = parameter;
            this.Body = body;

            this.HigherOrder = Factories.FromType(
                typeof(Func<,>).MakeGenericType(
                    ((Type)parameter.HigherOrder).Value.AsType(),
                    ((Type)body.HigherOrder).Value.AsType()));
        }

        public override Expression HigherOrder { get; }

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Body.GetHashCode();

        public bool Equals(Lambda? other) =>
            (other?.Parameter.Equals(this.Parameter) ?? false) &&
            (other?.Body.Equals(this.Body) ?? false);

        public override bool Equals(Expression? other) =>
            this.Equals(other as Lambda);

        public override string ToString() =>
            this.Parameter is Lambda ?
                $"({this.Parameter}) -> {this.Body}" :
                $"{this.Parameter} -> {this.Body}";

        public override Expression Run() =>
            Factories.Value(new Func<object, object>(parameter => this.Body.Run()));
    }
}
