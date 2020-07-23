﻿using Favalet.Expressions;
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
        public static IdentityTerm Identity(string symbol, IExpression higherOrder) =>
            IdentityTerm.Create(symbol, higherOrder);

        public static LogicalExpression Logical(IBinaryExpression operand) =>
            LogicalExpression.Create(operand);
        public static LogicalOperator Logical() =>
            LogicalOperator.Instance;

        public static AndExpression And(IExpression lhs, IExpression rhs) =>
            AndExpression.Create(lhs, rhs);
        public static AndExpression And(IExpression lhs, IExpression rhs, IExpression higherOrder) =>
            AndExpression.Create(lhs, rhs, higherOrder);

        public static OrExpression Or(IExpression lhs, IExpression rhs) =>
            OrExpression.Create(lhs, rhs);
        public static OrExpression Or(IExpression lhs, IExpression rhs, IExpression higherOrder) =>
            OrExpression.Create(lhs, rhs, higherOrder);

        public static LambdaExpression Lambda(
            IIdentityTerm parameter, IExpression body) =>
            LambdaExpression.Create(parameter, body);
        public static LambdaExpression Lambda(
            string parameter, IExpression body) =>
            LambdaExpression.Create(IdentityTerm.Create(parameter), body);

        public static ApplyExpression Apply(
            IExpression function, IExpression argument) =>
            ApplyExpression.Create(function, argument);

        public static FunctionExpression Function(
            IExpression parameter, IExpression result) =>
            FunctionExpression.Create(parameter, result);
    }
}
