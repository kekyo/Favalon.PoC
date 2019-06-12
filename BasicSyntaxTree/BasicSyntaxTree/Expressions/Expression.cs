using BasicSyntaxTree.Untyped;
using BasicSyntaxTree.Untyped.Types;
using System.Collections.Generic;
using System.Linq;
using BasicSyntaxTree.Expressions.Unresolved;

namespace BasicSyntaxTree.Expressions
{
    public abstract class Expression
    {
        public readonly TextRegion TextRegion;

        private protected Expression(TextRegion textRegion) =>
            this.TextRegion = textRegion;

        public abstract bool IsResolved { get; }

        internal abstract bool IsSafePrintable { get; }

        internal string SafePrintable =>
            this.IsSafePrintable ? this.ToString() : $"({this})";

        public static IReadOnlyDictionary<string, UntypedFunctionType> CreateFunctionTypeEnvironment(
            params (string name, UntypedFunctionType type)[] environments) =>
            environments.ToDictionary(entry => entry.name, entry => entry.type);

        public static IReadOnlyDictionary<string, UntypedType> CreateTypeEnvironment(
            params (string name, UntypedType type)[] environments) =>
            environments.ToDictionary(entry => entry.name, entry => entry.type);

        // =======================================================================

        public static UntypedConstantExpression Constant(object value, TextRegion textRegion) =>
            new UntypedConstantExpression(value, textRegion);

        public static UntypedVariableExpression Variable(string name, TextRegion textRegion) =>
            new UntypedVariableExpression(name, default, textRegion);
        public static UntypedVariableExpression Variable(string name, UntypedType annotatedType, TextRegion textRegion) =>
            new UntypedVariableExpression(name, annotatedType, textRegion);

        public static UntypedLambdaExpression Lambda(UntypedVariableExpression parameter, UntypedExpression body, TextRegion textRegion) =>
            new UntypedLambdaExpression(parameter, body, default, textRegion);
        public static UntypedLambdaExpression Lambda(UntypedVariableExpression parameter, UntypedExpression body, UntypedType annotatedType, TextRegion textRegion) =>
            new UntypedLambdaExpression(parameter, body, annotatedType, textRegion);

        public static UntypedApplyExpression Apply(UntypedExpression function, UntypedExpression argument, TextRegion textRegion) =>
            new UntypedApplyExpression(function, argument, default, textRegion);
        public static UntypedApplyExpression Apply(UntypedExpression function, UntypedExpression argument, UntypedType annotatedType, TextRegion textRegion) =>
            new UntypedApplyExpression(function, argument, annotatedType, textRegion);

        public static UntypedBindExpression Bind(UntypedVariableExpression target, UntypedExpression expression, UntypedExpression body, TextRegion textRegion) =>
            new UntypedBindExpression(target, expression, body, default, textRegion);
        public static UntypedBindExpression Bind(UntypedVariableExpression target, UntypedExpression expression, UntypedExpression body, UntypedType annotatedType, TextRegion textRegion) =>
            new UntypedBindExpression(target, expression, body, annotatedType, textRegion);
    }
}
