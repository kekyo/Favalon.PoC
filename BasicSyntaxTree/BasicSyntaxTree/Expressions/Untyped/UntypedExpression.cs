using BasicSyntaxTree.Expressions.Typed;
using BasicSyntaxTree.Types;
using System.Collections.Generic;

namespace BasicSyntaxTree.Expressions.Untyped
{
    public abstract class UntypedExpression : Expression
    {
        private protected UntypedExpression() { }

        internal abstract TypedExpression Visit(TypeEnvironment environment, InferContext context);

        public TypedExpression Infer<T>(T typeEnvironment) where T : IReadOnlyDictionary<string, Type>
        {
            var context = new InferContext();
            var typedExpression = this.Visit(new TypeEnvironment(typeEnvironment), context);
            typedExpression.Resolve(context);
            return typedExpression;
        }

        // =======================================================================

        public static UntypedConstantExpression Constant(object value) =>
            new UntypedConstantExpression(value);

        public static UntypedVariableExpression Variable(string name) =>
            new UntypedVariableExpression(name);

        public static UntypedLambdaExpression Lambda(string parameter, UntypedExpression body) =>
            new UntypedLambdaExpression(parameter, body);

        public static UntypedApplyExpression Apply(UntypedExpression function, UntypedExpression argument) =>
            new UntypedApplyExpression(function, argument);
    }
}
