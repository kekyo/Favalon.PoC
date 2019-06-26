namespace Favalon.Expressions
{
    partial class Expression
    {
        public static ConstantExpression Constant(object value, TextRange textRange) =>
            new ConstantExpression(value, textRange);

        public static FreeVariableExpression Variable(string name, TextRange textRange) =>
            new FreeVariableExpression(name, textRange);
        public static FreeVariableExpression Variable(string name, Expression higherOrder, TextRange textRange) =>
            new FreeVariableExpression(name, higherOrder, textRange);

        public static TypeExpression Type(string name, TextRange textRange) =>
            new TypeExpression(name, textRange);
        public static TypeExpression Type(System.Type type, TextRange textRange) =>
            new TypeExpression(type, textRange);

        public static KindExpression Kind() =>
            KindExpression.Instance;

        public static ApplyExpression Apply(Expression function, Expression argument, TextRange textRange) =>
            new ApplyExpression(function, argument, textRange);

        public static BindExpression Bind(
            FreeVariableExpression variable, Expression expression, Expression body, TextRange textRange) =>
            new BindExpression(variable, expression, body, textRange);

        public static LambdaExpression Lambda(
            Expression parameter, Expression expression, TextRange textRange) =>
            new LambdaExpression(parameter, expression, textRange);
    }
}
