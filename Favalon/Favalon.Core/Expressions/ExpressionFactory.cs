namespace Favalon.Expressions
{
    partial class Expression
    {
        public static ConstantExpression Constant(object value, TextRange textRange) =>
            new ConstantExpression(value, textRange);

        public static VariableExpression Variable(string name, TextRange textRange) =>
            new VariableExpression(name, textRange);
        public static VariableExpression Variable(string name, Expression higherOrder, TextRange textRange) =>
            new VariableExpression(name, higherOrder, textRange);

        public static TypeExpression Type(string name, TextRange textRange) =>
            new TypeExpression(name, textRange);
        public static TypeExpression Type(System.Type type, TextRange textRange) =>
            new TypeExpression(type, textRange);

        public static KindExpression Kind() =>
            KindExpression.Instance;

        public static ApplyExpression<TFunctionExpression, TArgumentExpression> Apply<TFunctionExpression, TArgumentExpression>(
            TFunctionExpression function, TArgumentExpression argument, TextRange textRange)
            where TFunctionExpression : Expression<TFunctionExpression>
            where TArgumentExpression : Expression<TArgumentExpression> =>
            new ApplyExpression<TFunctionExpression, TArgumentExpression>(function, argument, textRange);

        public static BindExpression<TExpressionExpression, TBodyExpression> Bind<TExpressionExpression, TBodyExpression>(
            VariableExpression variable, TExpressionExpression expression, TBodyExpression body, TextRange textRange)
            where TExpressionExpression : Expression<TExpressionExpression>
            where TBodyExpression : Expression<TBodyExpression> =>
            new BindExpression<TExpressionExpression, TBodyExpression>(variable, expression, body, textRange);

        public static LambdaExpression<TParameterExpression, TExpressionExpression> Lambda<TParameterExpression, TExpressionExpression>(
            TParameterExpression parameter, TExpressionExpression expression, TextRange textRange)
            where TParameterExpression : Expression<TParameterExpression>, IVariableExpression
            where TExpressionExpression : Expression<TExpressionExpression> =>
            new LambdaExpression<TParameterExpression, TExpressionExpression>(parameter, expression, textRange);
    }
}
