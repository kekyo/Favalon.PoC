namespace Favalon.Expressions
{
    partial class Expression
    {
        public static IntegerExpression Integer(int value, TextRange textRange) =>
            new IntegerExpression(value, textRange);

        public static VariableExpression Variable(string name, TextRange textRange) =>
            new VariableExpression(name, textRange);
        public static VariableExpression Variable(string name, Expression higherOrder, TextRange textRange) =>
            new VariableExpression(name, higherOrder, textRange);

        public static TypeExpression Type(string name, TextRange textRange) =>
            new TypeExpression(name, textRange);

        public static KindExpression Kind() =>
            KindExpression.Instance;

        public static ApplyExpression Apply(Expression function, Expression argument, TextRange textRange) =>
            new ApplyExpression(function, argument, textRange);

        public static BindExpression Bind(VariableExpression variable, Expression expression, Expression body, TextRange textRange) =>
            new BindExpression(variable, expression, body, textRange);

        public static LambdaExpression Lambda(IdentityExpression parameter, Expression expression, TextRange textRange) =>
            new LambdaExpression(parameter, expression, textRange);
    }
}
