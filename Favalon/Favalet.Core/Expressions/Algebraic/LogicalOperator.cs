﻿using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;

namespace Favalet.Expressions.Algebraic
{
    public interface ILogicalOperator : ICallableExpression
    {
    }

    public sealed class LogicalOperator :
        Expression, ILogicalOperator
    {
        private static readonly LogicalCalculator calculator = new LogicalCalculator();

        private LogicalOperator()
        { }

        public IExpression HigherOrder =>
            UnspecifiedTerm.Function;

        public bool Equals(ILogicalOperator rhs) =>
            true;

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is ILogicalOperator rhs && this.Equals(rhs);

        public IExpression Infer(IReduceContext context) =>
            this;

        public IExpression Reduce(IReduceContext context) =>
            this;

        public IExpression Call(IReduceContext context, IExpression argument) =>
            calculator.Compute(argument);

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                "Logical");

        public static readonly LogicalOperator Instance =
            new LogicalOperator();
    }
}
