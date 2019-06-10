using BasicSyntaxTree.Typed;
using BasicSyntaxTree.Untyped.Expressions;

namespace BasicSyntaxTree.Untyped
{
    public abstract class UntypedExpression : Expression
    {
        private protected UntypedExpression(UntypedType? annotatedType, TextRegion textRegion) : base(textRegion) =>
            this.AnnotetedType = annotatedType;

        public override bool IsResolved => false;

        public UntypedType? AnnotetedType { get; }

        internal abstract TypedExpression Visit(Environment environment, InferContext context);

        public TypedExpression Infer(Environment typeEnvironment)
        {
            var context = new InferContext();
            var typedExpression = this.Visit(typeEnvironment, context);
            typedExpression.Resolve(context);
            return typedExpression;
        }

        // =======================================================================
        // Short generator usable for tests.

        public static UntypedConstantExpression Constant(object value) =>
            new UntypedConstantExpression(value, TextRegion.Unknown);

        public static UntypedVariableExpression Variable(string name) =>
            new UntypedVariableExpression(name, default, TextRegion.Unknown);
        public static UntypedVariableExpression Variable(string name, UntypedType annotatedType) =>
            new UntypedVariableExpression(name, annotatedType, TextRegion.Unknown);

        public static UntypedLambdaExpression Lambda(UntypedVariableExpression parameter, UntypedExpression body, UntypedType? annotatedType = default) =>
            new UntypedLambdaExpression(parameter, body, annotatedType, TextRegion.Unknown);

        public static UntypedApplyExpression Apply(UntypedExpression function, UntypedExpression argument, UntypedType? annotatedType = default) =>
            new UntypedApplyExpression(function, argument, annotatedType, TextRegion.Unknown);

        public static UntypedBindExpression Bind(UntypedVariableExpression target, UntypedExpression expression, UntypedExpression body, UntypedType? annotatedType = default) =>
            new UntypedBindExpression(target, expression, body, annotatedType, TextRegion.Unknown);

        public static implicit operator UntypedExpression(string variableName) =>
            new UntypedVariableExpression(variableName, default, TextRegion.Unknown);
    }
}
