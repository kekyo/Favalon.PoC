namespace Favalon.Expressions.Internals
{
    public static class StaticFactory
    {
        public static IntegerExpression Integer(int value) =>
            new IntegerExpression(value);

        public static VariableExpression Variable(string name) =>
            new VariableExpression(name);
        public static VariableExpression Variable(string name, Expression higherOrder) =>
            new VariableExpression(name, higherOrder);

        public static FreeVariableExpression FreeVariable(this Environment environment) =>
            environment.CreateFreeVariable();

        public static TypeExpression Type(string name) =>
            new TypeExpression(name);

        public static KindExpression Kind() =>
            KindExpression.Instance;

        public static ApplyExpression Apply(Expression function, Expression argument) =>
            new ApplyExpression(function, argument);

        public static BindExpression Bind(VariableExpression variable, Expression expression, Expression body) =>
            new BindExpression(variable, expression, body);

        public static LambdaExpression Lambda(IdentityExpression parameter, Expression expression) =>
            new LambdaExpression(parameter, expression);
    }
}
