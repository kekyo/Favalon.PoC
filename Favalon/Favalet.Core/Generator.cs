using Favalet.Expressions;
using Favalet.Expressions.Algebraic;

namespace Favalet
{
    public static class Generator
    {
        public static IdentityTerm Identity(string symbol) =>
            IdentityTerm.Create(symbol);

        public static LogicalOperator Logical(IBinaryExpression operand) =>
            LogicalOperator.Create(operand);

        public static AndBinaryExpression AndBinary(
            IExpression lhs, IExpression rhs) =>
            AndBinaryExpression.Create(lhs, rhs);

        public static OrBinaryExpression OrBinary(
            IExpression lhs, IExpression rhs) =>
            OrBinaryExpression.Create(lhs, rhs);

        public static AndExpression And(params IExpression[] operands) =>
            AndExpression.Create(operands);

        public static OrExpression Or(params IExpression[] operands) =>
            OrExpression.Create(operands);
    }
}
