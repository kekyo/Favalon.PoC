namespace Favalon.Expressions.Internals
{
    public static class StaticFactory
    {
        public static ConstantExpression Constant(object value) =>
            new ConstantExpression(value, TextRange.Unknown);

        public static FreeVariableExpression Variable(string name) =>
            new FreeVariableExpression(name, TextRange.Unknown);
        public static FreeVariableExpression Variable(string name, Expression higherOrder) =>
            new FreeVariableExpression(name, higherOrder, TextRange.Unknown);

        public static PlaceholderExpression Placeholder(this Environment environment) =>
            environment.CreatePlaceholder(UndefinedExpression.Instance, TextRange.Unknown);
        public static PlaceholderExpression Placeholder(this Environment environment, Expression higherOrder) =>
            environment.CreatePlaceholder(higherOrder, TextRange.Unknown);

        public static TypeExpression Type(string name) =>
            new TypeExpression(name, TextRange.Unknown);
        public static TypeExpression Type(System.Type type) =>
            new TypeExpression(type, TextRange.Unknown);

        public static KindExpression Kind() =>
            KindExpression.Instance;

        public static ApplyExpression Apply(Expression function, Expression argument) =>
            new ApplyExpression(function, argument, TextRange.Unknown);

        public static BindExpression Bind(FreeVariableExpression variable, Expression expression, Expression body) =>
            new BindExpression(variable, expression, body, TextRange.Unknown);

        public static LambdaExpression Lambda(Expression parameter, Expression expression) =>
            new LambdaExpression(parameter, expression, TextRange.Unknown);

        public static NewExpression New(IdentityExpression argument) =>
            new NewExpression(argument, TextRange.Unknown);
    }
}
