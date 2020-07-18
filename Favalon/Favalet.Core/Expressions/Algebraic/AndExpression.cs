using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IAndExpression :
        ISetExpression
    {
    }

    public sealed class AndExpression :
        SetExpression<AndExpression>, IAndExpression
    {
        private AndExpression(IExpression[] operands) :
            base(operands)
        { }

        public override IExpression Reduce(IReduceContext context)
        {
            var operands = this.Operands.
                Select(operand => operand.Reduce(context)).
                ToArray();

            if (this.Operands.SequenceEqual(operands, ReferenceComparer.Instance))
            {
                return this;
            }
            else
            {
                return new AndExpression(operands);
            }
        }

        public static AndExpression Create(IExpression[] operands) =>
            new AndExpression(operands);
    }
}
