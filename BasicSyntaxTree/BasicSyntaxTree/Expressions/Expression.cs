using System.Collections.Generic;
using System.Linq;
using BasicSyntaxTree.Expressions.Unresolved;
using BasicSyntaxTree.Types;
using BasicSyntaxTree.Types.Unresolved;

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

        public static IReadOnlyDictionary<string, UnresolvedFunctionType> CreateFunctionTypeEnvironment(
            params (string name, UnresolvedFunctionType type)[] environments) =>
            environments.ToDictionary(entry => entry.name, entry => entry.type);

        public static IReadOnlyDictionary<string, UnresolvedType> CreateTypeEnvironment(
            params (string name, UnresolvedType type)[] environments) =>
            environments.ToDictionary(entry => entry.name, entry => entry.type);

        // =======================================================================

        public static UnresolvedConstantExpression Constant(object value, TextRegion textRegion) =>
            new UnresolvedConstantExpression(value, textRegion);

        public static UnresolvedVariableExpression Variable(string name, TextRegion textRegion) =>
            new UnresolvedVariableExpression(name, default, textRegion);
        public static UnresolvedVariableExpression Variable(string name, UnresolvedType annotatedType, TextRegion textRegion) =>
            new UnresolvedVariableExpression(name, annotatedType, textRegion);

        public static UnresolvedLambdaExpression Lambda(UnresolvedVariableExpression parameter, UnresolvedExpression body, TextRegion textRegion) =>
            new UnresolvedLambdaExpression(parameter, body, default, textRegion);
        public static UnresolvedLambdaExpression Lambda(UnresolvedVariableExpression parameter, UnresolvedExpression body, UnresolvedType annotatedType, TextRegion textRegion) =>
            new UnresolvedLambdaExpression(parameter, body, annotatedType, textRegion);

        public static UnresolvedApplyExpression Apply(UnresolvedExpression function, UnresolvedExpression argument, TextRegion textRegion) =>
            new UnresolvedApplyExpression(function, argument, default, textRegion);
        public static UnresolvedApplyExpression Apply(UnresolvedExpression function, UnresolvedExpression argument, UnresolvedType annotatedType, TextRegion textRegion) =>
            new UnresolvedApplyExpression(function, argument, annotatedType, textRegion);

        public static UnresolvedBindExpression Bind(UnresolvedVariableExpression target, UnresolvedExpression expression, UnresolvedExpression body, TextRegion textRegion) =>
            new UnresolvedBindExpression(target, expression, body, default, textRegion);
        public static UnresolvedBindExpression Bind(UnresolvedVariableExpression target, UnresolvedExpression expression, UnresolvedExpression body, UnresolvedType annotatedType, TextRegion textRegion) =>
            new UnresolvedBindExpression(target, expression, body, annotatedType, textRegion);
    }
}
