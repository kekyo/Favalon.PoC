using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet
{
    public static class Generator
    {
        public static IdentityTerm Identity(string symbol) =>
            IdentityTerm.Create(symbol);

        public static EquivalenceExpression Equivalence(IOperandExpression operand) =>
            EquivalenceExpression.Create(operand);

        public static AndExpression And(params IExpression[] operands) =>
            AndExpression.Create(operands);

        public static OrExpression Or(params IExpression[] operands) =>
            OrExpression.Create(operands);
    }
}
