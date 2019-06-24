namespace Favalon.Expressions.Internals
{
    public static class StaticFactory
    {
        public static IntegerExpression Integer(int value) =>
            new IntegerExpression(value);

        public static VariableExpression Variable(string name) =>
            new VariableExpression(name);

        public static TypeExpression Type(string name) =>
            new TypeExpression(name);

        public static ApplyExpression Apply(Expression function, Expression argument) =>
            new ApplyExpression(function, argument);

        public static BindExpression Bind(VariableExpression variable, Expression expression, Expression body) =>
            new BindExpression(variable, expression, body);

        public static LambdaExpression Lambda(VariableExpression parameter, Expression expression) =>
            new LambdaExpression(parameter, expression);
    }
}
