using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet
{
    public interface IAndExpression : IExpression
    {
    }

    public sealed class AndExpression : IAndExpression
    {
        private readonly IExpression[] Operands;

        private AndExpression(IExpression[] operands) =>
            this.Operands = operands;

        public static AndExpression Create(IExpression[] operands) =>
            new AndExpression(operands);
    }
}
