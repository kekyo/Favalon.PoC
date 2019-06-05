using BasicSyntaxTree.Expressions.Typed;
using BasicSyntaxTree.Types;
using System.Collections.Generic;

namespace BasicSyntaxTree.Expressions.Untyped
{
    public abstract class UntypedExpression : Expression
    {
        private protected UntypedExpression(TextRegion textRegion) : base(textRegion) { }

        internal abstract TypedExpression Visit(TypeEnvironment environment, InferContext context);

        public TypedExpression Infer<T>(T typeEnvironment) where T : IReadOnlyDictionary<string, Type>
        {
            var context = new InferContext();
            var typedExpression = this.Visit(new TypeEnvironment(typeEnvironment), context);
            typedExpression.Resolve(context);
            return typedExpression;
        }

        // =======================================================================

        public static UntypedConstantExpression Constant(object value, TextRegion textRegion) =>
            new UntypedConstantExpression(value, textRegion);

        public static UntypedVariableExpression Variable(string name, TextRegion textRegion) =>
            new UntypedVariableExpression(name, textRegion);

        public static UntypedLambdaExpression Lambda(string parameter, UntypedExpression body, TextRegion textRegion) =>
            new UntypedLambdaExpression(parameter, body, textRegion);

        public static UntypedApplyExpression Apply(UntypedExpression function, UntypedExpression argument, TextRegion textRegion) =>
            new UntypedApplyExpression(function, argument, textRegion);
    }
}
