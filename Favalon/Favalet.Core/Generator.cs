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

        public static AndBinaryExpression AndBinary(
            IExpression lhs, IExpression rhs, params IExpression[] operands) =>
            operands.Aggregate(
                AndBinaryExpression.Create(lhs, rhs),
                AndBinaryExpression.Create);

        public static OrBinaryExpression OrBinary(
            IExpression lhs, IExpression rhs, params IExpression[] operands) =>
            operands.Aggregate(
                OrBinaryExpression.Create(lhs, rhs),
                OrBinaryExpression.Create);
    }
}
