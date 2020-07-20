using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using System;
using System.Linq;

namespace Favalet
{
    public static class Generator
    {
        public static IdentityTerm Identity(string symbol) =>
            IdentityTerm.Create(symbol);

        public static LogicalOperator Logical(IBinaryExpression operand) =>
            LogicalOperator.Create(operand);

        public static AndExpression And(
            IExpression lhs, IExpression rhs, params IExpression[] operands) =>
            operands.Aggregate(
                AndExpression.Create(lhs, rhs),
                AndExpression.Create);

        public static OrExpression Or(
            IExpression lhs, IExpression rhs, params IExpression[] operands) =>
            operands.Aggregate(
                OrExpression.Create(lhs, rhs),
                OrExpression.Create);
    }
}
