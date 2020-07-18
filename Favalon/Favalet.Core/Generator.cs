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

        public static LogicalOperator Logical(IBinaryExpression operand) =>
            LogicalOperator.Create(operand);

        public static AndBinaryExpression AndBinary(
            IExpression operand0, IExpression operand1, params IExpression[] operands) =>
            operands.Aggregate(
                AndBinaryExpression.Create(operand0, operand1),
                AndBinaryExpression.Create);

        public static OrBinaryExpression OrBinary(
            IExpression operand0, IExpression operand1, params IExpression[] operands) =>
            operands.Aggregate(
                OrBinaryExpression.Create(operand0, operand1),
                OrBinaryExpression.Create);

        public static AndExpression And(params IExpression[] operands) =>
            AndExpression.Create(operands);

        public static OrExpression Or(params IExpression[] operands) =>
            OrExpression.Create(operands);
    }
}
