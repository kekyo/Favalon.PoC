using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System;
using System.Linq;

namespace Favalet
{
    public static class Generator
    {
        public static FourthTerm Fourth() =>
            FourthTerm.Instance;

        private static readonly IdentityTerm kind =
            IdentityTerm.Create("*", FourthTerm.Instance);
        public static IdentityTerm Kind() =>
            kind;

        public static IdentityTerm Identity(string symbol) =>
            IdentityTerm.Create(symbol);

        public static LogicalExpression Logical(IBinaryExpression operand) =>
            LogicalExpression.Create(operand);
        public static LogicalOperator Logical() =>
            LogicalOperator.Instance;

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

        public static LambdaExpression Lambda(
            string parameter, IExpression body) =>
            LambdaExpression.Create(parameter, body);

        public static ApplyExpression Apply(
            IExpression function, IExpression argument) =>
            ApplyExpression.Create(function, argument);

        public static FunctionExpression Function(
            IExpression parameter, IExpression result) =>
            FunctionExpression.Create(parameter, result);
    }
}
