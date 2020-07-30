using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Diagnostics;

namespace Favalet
{
    public static class Generator
    {
        [DebuggerStepThrough]
        public static Environments Environment() =>
            Favalet.Environments.Create(LogicalCalculator.Instance);

        private static readonly IdentityTerm kind =
            IdentityTerm.Create("*", FourthTerm.Instance);

        [DebuggerStepThrough]
        public static IdentityTerm Kind() =>
            kind;
        [DebuggerStepThrough]
        public static IdentityTerm Kind(string symbol) =>
            IdentityTerm.Create(symbol, FourthTerm.Instance);

        [DebuggerStepThrough]
        public static UnspecifiedTerm Unspecified() =>
            UnspecifiedTerm.Instance;

        [DebuggerStepThrough]
        public static IdentityTerm Identity(string symbol) =>
            IdentityTerm.Create(symbol);
        [DebuggerStepThrough]
        public static IdentityTerm Identity(string symbol, IExpression higherOrder) =>
            IdentityTerm.Create(symbol, higherOrder);

        [DebuggerStepThrough]
        public static BoundSymbolTerm BoundSymbol(string symbol) =>
            BoundSymbolTerm.Create(symbol);
        [DebuggerStepThrough]
        public static BoundSymbolTerm BoundSymbol(string symbol, IExpression higherOrder) =>
            BoundSymbolTerm.Create(symbol, higherOrder);

        [DebuggerStepThrough]
        public static LogicalExpression Logical(IBinaryExpression operand) =>
            LogicalExpression.Create(operand);
        [DebuggerStepThrough]
        public static LogicalOperator Logical() =>
            LogicalOperator.Instance;

        [DebuggerStepThrough]
        public static AndExpression And(IExpression lhs, IExpression rhs) =>
            AndExpression.Create(lhs, rhs);
        [DebuggerStepThrough]
        public static AndExpression And(IExpression lhs, IExpression rhs, IExpression higherOrder) =>
            AndExpression.Create(lhs, rhs, higherOrder);

        [DebuggerStepThrough]
        public static OrExpression Or(IExpression lhs, IExpression rhs) =>
            OrExpression.Create(lhs, rhs);
        [DebuggerStepThrough]
        public static OrExpression Or(IExpression lhs, IExpression rhs, IExpression higherOrder) =>
            OrExpression.Create(lhs, rhs, higherOrder);

        [DebuggerStepThrough]
        public static LambdaExpression Lambda(
            IBoundSymbolTerm parameter, IExpression body) =>
            LambdaExpression.Create(parameter, body);
        [DebuggerStepThrough]
        public static LambdaExpression Lambda(
            string parameter, IExpression body) =>
            LambdaExpression.Create(BoundSymbolTerm.Create(parameter), body);
        [DebuggerStepThrough]
        public static LambdaExpression Lambda(
            IBoundSymbolTerm parameter, IExpression body, IExpression higherOrder) =>
            LambdaExpression.Create(parameter, body, higherOrder);
        [DebuggerStepThrough]
        public static LambdaExpression Lambda(
            string parameter, IExpression body, IExpression higherOrder) =>
            LambdaExpression.Create(BoundSymbolTerm.Create(parameter), body, higherOrder);

        [DebuggerStepThrough]
        public static ApplyExpression Apply(
            IExpression function, IExpression argument) =>
            ApplyExpression.Create(function, argument);

        [DebuggerStepThrough]
        public static IExpression Function(
            IExpression parameter, IExpression result) =>
            FunctionExpression.From(parameter, result);
        [DebuggerStepThrough]
        public static IExpression Function(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            FunctionExpression.From(parameter, result, higherOrder);
    }
}
