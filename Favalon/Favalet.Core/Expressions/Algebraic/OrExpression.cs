using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IOrExpression :
        ISetExpression
    {
    }

    public sealed class OrExpression :
        SetExpression<OrExpression>, IOrExpression
    {
        private OrExpression(IExpression[] operands) :
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
                return new OrExpression(operands);
            }
        }

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple =>
                    "(" + string.Join(" || ", this.Operands.Select(oper => oper.GetPrettyString(type))) + ")",
                _ =>
                    "(Or " + string.Join(" ", this.Operands.Select(oper => oper.GetPrettyString(type))) + ")",
            };

        public static OrExpression Create(IExpression[] operands) =>
            new OrExpression(operands);
    }
}
