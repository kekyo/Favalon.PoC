using BasicSyntaxTree.Untyped.Types;
using BasicSyntaxTree.Untyped.Expressions;
using System.Collections.Generic;
using System.Linq;
using BasicSyntaxTree.Untyped;

namespace BasicSyntaxTree
{
    public abstract class Expression
    {
        public readonly TextRegion TextRegion;

        protected Expression(TextRegion textRegion) =>
            this.TextRegion = textRegion;

        public abstract bool IsResolved { get; }

        public static IReadOnlyDictionary<string, UntypedType> CreateEnvironment(
            params (string name, UntypedType type)[] environments) =>
            environments.ToDictionary(entry => entry.name, entry => entry.type);

        // =======================================================================

        public static UntypedConstantExpression Constant(object value, TextRegion textRegion) =>
            new UntypedConstantExpression(value, textRegion);

        public static UntypedVariableExpression Variable(string name, TextRegion textRegion) =>
            new UntypedVariableExpression(name, textRegion);

        public static UntypedLambdaExpression Lambda(string parameter, UntypedExpression body, TextRegion textRegion) =>
            new UntypedLambdaExpression(parameter, body, textRegion);

        public static UntypedApplyExpression Apply(UntypedExpression function, UntypedExpression argument, TextRegion textRegion) =>
            new UntypedApplyExpression(function, argument, textRegion);

        public static UntypedBindExpression Bind(string name, UntypedExpression expression, UntypedExpression body, TextRegion textRegion) =>
            new UntypedBindExpression(name, expression, body, textRegion);
    }
}
