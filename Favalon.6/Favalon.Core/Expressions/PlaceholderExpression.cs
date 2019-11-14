using System;

namespace Favalon.Expressions
{
    public sealed class PlaceholderExpression : Expression
    {
        public readonly int Id;

        public PlaceholderExpression(int id, Expression higherOrder)
        {
            this.Id = id;
            this.HigherOrder = higherOrder;
        }

        public override Expression HigherOrder { get; }

        protected override Expression VisitResolve(IInferContext context) =>
            this;

        public override bool Equals(Expression? rhs) =>
            rhs is PlaceholderExpression placeholder ?
                this.Id.Equals(placeholder.Id) :
                false;

        public override string ToString() =>
            $"'{this.Id}";
    }
}
