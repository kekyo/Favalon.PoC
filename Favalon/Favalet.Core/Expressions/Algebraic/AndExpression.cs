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
                Memoize();

            if (this.Operands.SequenceEqual(operands, ReferenceComparer.Instance))
            {
                return this;
            }
            else
            {
                return new AndExpression(operands);
            }
        }

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple =>
                    "(" + string.Join(" && ", this.Operands.Select(oper => oper.GetPrettyString(type))) + ")",
                _ =>
                    "(And " + string.Join(" ", this.Operands.Select(oper => oper.GetPrettyString(type))) + ")",
            };

        public static AndExpression Create(params IExpression[] operands) =>
            new AndExpression(operands);
    }
}
