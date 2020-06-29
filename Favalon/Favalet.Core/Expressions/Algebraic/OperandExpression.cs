using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IOperandExpression : IExpression
    {
        IExpression[] Operands { get; }
    }

    public abstract class OperandExpression<TOperandExpression> :
        IOperandExpression
        where TOperandExpression : IOperandExpression
    {
        public readonly IExpression[] Operands;

        protected OperandExpression(IExpression[] operands) =>
            this.Operands = operands;

        IExpression[] IOperandExpression.Operands =>
            this.Operands;

        public override int GetHashCode() =>
            this.Operands.Aggregate(0, (agg, v) => v.GetHashCode());

        public bool Equals(TOperandExpression rhs) =>
            this.Operands.SequenceEqual(rhs.Operands);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is TOperandExpression rhs && Equals(rhs);

        public IExpression Reduce(IReduceContext context)
        {
            return this;
        }
    }
}
