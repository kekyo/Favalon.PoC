namespace Favalon.Expressions.Internals
{
    public static class StaticFactory
    {
        public static IntegerExpression Integer(int value) =>
            new IntegerExpression(value, TextRange.Unknown);

        public static VariableExpression Variable(string name) =>
            new VariableExpression(name, TextRange.Unknown);
        public static VariableExpression Variable(string name, Expression higherOrder) =>
            new VariableExpression(name, higherOrder, TextRange.Unknown);

        public static FreeVariableExpression FreeVariable(this ExpressionEnvironment environment) =>
            environment.CreateFreeVariable(TextRange.Unknown);

        public static TypeExpression Type(string name) =>
            new TypeExpression(name, TextRange.Unknown);

        public static KindExpression Kind() =>
            KindExpression.Instance;

        public static ApplyExpression Apply(Expression function, Expression argument) =>
            new ApplyExpression(function, argument, TextRange.Unknown);

        public static BindExpression Bind(VariableExpression variable, Expression expression, Expression body) =>
            new BindExpression(variable, expression, body, TextRange.Unknown);

        public static LambdaExpression Lambda(IdentityExpression parameter, Expression expression) =>
            new LambdaExpression(parameter, expression, TextRange.Unknown);
    }
}
