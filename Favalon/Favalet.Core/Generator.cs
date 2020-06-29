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

        public static EquivalenceOperator Equivalence(IBinaryExpression operand) =>
            EquivalenceOperator.Create(operand);

        public static AndExpression And(params IExpression[] operands) =>
            (AndExpression)operands.Aggregate(AndExpression.Create);

        public static OrExpression Or(params IExpression[] operands) =>
            (OrExpression)operands.Aggregate(OrExpression.Create);
    }
}
