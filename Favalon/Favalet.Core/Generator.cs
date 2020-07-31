using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Favalet
{
    [DebuggerStepThrough]
    public static class Generator
    {
        public static Environments Environment() =>
            Favalet.Environments.Create(LogicalCalculator.Instance);

        private static readonly IdentityTerm kind =
            IdentityTerm.Create("*", FourthTerm.Instance);

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IdentityTerm Kind() =>
            kind;
        public static IdentityTerm Kind(string symbol) =>
            IdentityTerm.Create(symbol, FourthTerm.Instance);

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static UnspecifiedTerm Unspecified() =>
            UnspecifiedTerm.Instance;

        public static IdentityTerm Identity(string symbol) =>
            IdentityTerm.Create(symbol);
        public static IdentityTerm Identity(string symbol, IExpression higherOrder) =>
            IdentityTerm.Create(symbol, higherOrder);

        public static BoundSymbolTerm BoundSymbol(string symbol) =>
            BoundSymbolTerm.Create(symbol);
        public static BoundSymbolTerm BoundSymbol(string symbol, IExpression higherOrder) =>
            BoundSymbolTerm.Create(symbol, higherOrder);

        public static LogicalExpression Logical(IBinaryExpression operand) =>
            LogicalExpression.Create(operand);
#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
            IBoundSymbolTerm parameter, IExpression body) =>
            LambdaExpression.Create(parameter, body);
        public static LambdaExpression Lambda(
            string parameter, IExpression body) =>
            LambdaExpression.Create(BoundSymbolTerm.Create(parameter), body);
        public static LambdaExpression Lambda(
            IBoundSymbolTerm parameter, IExpression body, IExpression higherOrder) =>
            LambdaExpression.Create(parameter, body, higherOrder);
        public static LambdaExpression Lambda(
            string parameter, IExpression body, IExpression higherOrder) =>
            LambdaExpression.Create(BoundSymbolTerm.Create(parameter), body, higherOrder);

        public static ApplyExpression Apply(
            IExpression function, IExpression argument) =>
            ApplyExpression.Create(function, argument);
        public static ApplyExpression Apply(
            IExpression function, IExpression argument, IExpression higherOrder) =>
            ApplyExpression.Create(function, argument, higherOrder);

        public static FunctionExpression Function(
            IExpression parameter, IExpression result) =>
            FunctionExpression.Create(parameter, result);
        public static FunctionExpression Function(
            IExpression parameter, IExpression result, IExpression higherOrder) =>
            FunctionExpression.Create(parameter, result, higherOrder);
    }
}
