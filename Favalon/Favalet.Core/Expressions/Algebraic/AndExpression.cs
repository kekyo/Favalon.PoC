using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IAndExpression : IExpression
    {
        IExpression[] Operands { get; }
    }

    public sealed class AndExpression : IAndExpression
    {
        public readonly IExpression[] Operands;

        private AndExpression(IExpression[] operands) =>
            this.Operands = operands;

        IExpression[] IAndExpression.Operands =>
            this.Operands;

        public override int GetHashCode() =>
            this.Operands.Aggregate(0, (agg, v) => v.GetHashCode());

        public bool Equals(IAndExpression rhs) =>
            this.Operands.SequenceEqual(rhs.Operands);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IAndExpression rhs && Equals(rhs);

        public IExpression Reduce(IReduceContext context)
        {
            return this;
        }

        public static AndExpression Create(IExpression[] operands) =>
            new AndExpression(operands);
    }
}
