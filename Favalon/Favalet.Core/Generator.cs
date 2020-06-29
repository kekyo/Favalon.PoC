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

        public static EquivalenceExpression Equivalence(IAndExpression expression) =>
            EquivalenceExpression.Create(expression);

        public static AndExpression And(params IExpression[] operands) =>
            AndExpression.Create(operands);
    }
}
