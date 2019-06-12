using BasicSyntaxTree.Expressions.Unresolved;
using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions
{
    public abstract class UnresolvedExpression : Expression
    {
        private protected UnresolvedExpression(Type? annotatedType, TextRegion textRegion) : base(textRegion) =>
            this.AnnotetedType = annotatedType;

        public override bool IsResolved => false;

        public Type? AnnotetedType { get; }

        internal abstract ResolvedExpression Visit(Environment environment, InferContext context);

        public ResolvedExpression Infer(Environment typeEnvironment)
        {
            var context = new InferContext();
            var typedExpression = this.Visit(typeEnvironment, context);
            typedExpression.Resolve(context);
            return typedExpression;
        }

        // =======================================================================
        // Short generator usable for tests.

        public static UnresolvedConstantExpression Constant(object value) =>
            new UnresolvedConstantExpression(value, TextRegion.Unknown);

        public static UnresolvedVariableExpression Variable(string name, Type? annotatedType = default) =>
            new UnresolvedVariableExpression(name, annotatedType, TextRegion.Unknown);

        public static UnresolvedLambdaExpression Lambda(UnresolvedVariableExpression parameter, UnresolvedExpression body, Type? annotatedType = default) =>
            new UnresolvedLambdaExpression(parameter, body, annotatedType, TextRegion.Unknown);

        public static UnresolvedApplyExpression Apply(UnresolvedExpression function, UnresolvedExpression argument, Type? annotatedType = default) =>
            new UnresolvedApplyExpression(function, argument, annotatedType, TextRegion.Unknown);

        public static UnresolvedBindExpression Bind(UnresolvedVariableExpression target, UnresolvedExpression expression, UnresolvedExpression body, Type? annotatedType = default) =>
            new UnresolvedBindExpression(target, expression, body, annotatedType, TextRegion.Unknown);

        public static implicit operator UnresolvedExpression(string variableName) =>
            new UnresolvedVariableExpression(variableName, default, TextRegion.Unknown);
    }
}
