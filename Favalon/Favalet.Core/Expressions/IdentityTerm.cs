using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface IIdentityTerm : ITerm
    {
        string Symbol { get; }
    }

    public sealed class IdentityTerm :
        Expression, IIdentityTerm
    {
        public readonly string Symbol;

        private IdentityTerm(string symbol, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Symbol = symbol;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string IIdentityTerm.Symbol =>
            this.Symbol;

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(IIdentityTerm rhs) =>
            this.Symbol.Equals(rhs.Symbol);

        public override bool Equals(IExpression? other) =>
            other is IIdentityTerm rhs && this.Equals(rhs);

        protected override IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);

            if (context.LookupVariable(this) is IExpression lookup)
            {
                var inferred = context.Infer(lookup);
                context.Unify(inferred.HigherOrder, higherOrder);
            }

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new IdentityTerm(this.Symbol, higherOrder);
            }
        }

        protected override IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.Fixup(this.HigherOrder);

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new IdentityTerm(this.Symbol, higherOrder);
            }
        }

        protected override IExpression Reduce(IReduceContext context)
        {
            if (context.LookupVariable(this) is IExpression lookup)
            {
                var reduced = context.Reduce(lookup);
                return context.TypeCalculator.Compute(reduced);
            }
            else
            {
                return this;
            }
        }

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    this.Symbol :
                    $"Identity {this.Symbol}");

        [DebuggerStepThrough]
        public static IdentityTerm Create(string symbol, IExpression higherOrder) =>
            new IdentityTerm(symbol, higherOrder);
        [DebuggerStepThrough]
        public static IdentityTerm Create(string symbol) =>
            new IdentityTerm(symbol, UnspecifiedTerm.Instance);
    }
}
