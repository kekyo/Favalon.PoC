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

        public IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string IIdentityTerm.Symbol =>
            this.Symbol;

        public override int GetHashCode() =>
            this.Symbol.GetHashCode();

        public bool Equals(IIdentityTerm rhs) =>
            this.Symbol.Equals(rhs.Symbol);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IIdentityTerm rhs && this.Equals(rhs);

        public IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);

            if (context.LookupVariable(this) is IExpression lookup)
            {
                var inferred = lookup.Infer(context);
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

        public IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.FixupHigherOrder(this.HigherOrder);

            if (object.ReferenceEquals(this.HigherOrder, higherOrder))
            {
                return this;
            }
            else
            {
                return new IdentityTerm(this.Symbol, higherOrder);
            }
        }

        public IExpression Reduce(IReduceContext context)
        {
            if (context.LookupVariable(this) is IExpression lookup)
            {
                var reduced = lookup.Reduce(context);
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

        public static IdentityTerm Create(string symbol, IExpression higherOrder) =>
            new IdentityTerm(symbol, higherOrder);
        public static IdentityTerm Create(string symbol) =>
            new IdentityTerm(symbol, UnspecifiedTerm.Instance);
    }
}
