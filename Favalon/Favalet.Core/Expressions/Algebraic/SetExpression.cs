using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface ISetExpression : IExpression
    {
        IExpression[] Operands { get; }
    }

    public abstract class SetExpression<TSetExpression> :
        Expression, ISetExpression
        where TSetExpression : ISetExpression
    {
        public readonly IExpression[] Operands;

        protected SetExpression(IExpression[] operands) =>
            this.Operands = operands;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression[] ISetExpression.Operands =>
            this.Operands;

        public override int GetHashCode() =>
            this.Operands.Aggregate(0, (agg, ex) => agg ^ ex.GetHashCode());

        public bool Equals(TSetExpression rhs) =>
            this.Operands.EqualsPartiallyOrdered(rhs.Operands);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is TSetExpression rhs && Equals(rhs);

        public abstract IExpression Reduce(IReduceContext context);
    }

    public static class SetExpressionExtension
    {
        public static void Deconstruct(
            this ISetExpression set,
            out IExpression[] operands) =>
            operands = set.Operands;
    }
}
