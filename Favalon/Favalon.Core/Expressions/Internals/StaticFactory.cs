namespace Favalon.Expressions.Internals
{
    public static class StaticFactory
    {
        public static ConstantExpression Constant(object value) =>
            new ConstantExpression(value, TextRange.Unknown);

        public static VariableExpression Variable(string name) =>
            new VariableExpression(name, TextRange.Unknown);
        public static VariableExpression Variable(string name, Expression higherOrder) =>
            new VariableExpression(name, higherOrder, TextRange.Unknown);

        public static FreeVariableExpression FreeVariable(this ExpressionEnvironment environment) =>
            environment.CreateFreeVariable(TextRange.Unknown);

        public static TypeExpression Type(string name) =>
            new TypeExpression(name, TextRange.Unknown);
        public static TypeExpression Type(System.Type type) =>
            new TypeExpression(type, TextRange.Unknown);

        public static KindExpression Kind() =>
            KindExpression.Instance;

        public static ApplyExpression<TFunctionExpression, TArgumentExpression> Apply<TFunctionExpression, TArgumentExpression>(
            TFunctionExpression function, TArgumentExpression argument)
            where TFunctionExpression : Expression<TFunctionExpression>
            where TArgumentExpression : Expression<TArgumentExpression> =>
            new ApplyExpression<TFunctionExpression, TArgumentExpression>(function, argument, TextRange.Unknown);

        public static BindExpression<TExpressionExpression, TBodyExpression> Bind<TExpressionExpression, TBodyExpression>(
            VariableExpression variable, TExpressionExpression expression, TBodyExpression body)
            where TExpressionExpression : Expression<TExpressionExpression>
            where TBodyExpression : Expression<TBodyExpression> =>
            new BindExpression<TExpressionExpression, TBodyExpression>(variable, expression, body, TextRange.Unknown);

        public static LambdaExpression<TParameterExpression, TExpressionExpression> Lambda<TParameterExpression, TExpressionExpression>(
            TParameterExpression parameter, TExpressionExpression expression)
            where TParameterExpression : Expression<TParameterExpression>, IVariableExpression
            where TExpressionExpression : Expression<TExpressionExpression> =>
            new LambdaExpression<TParameterExpression, TExpressionExpression>(parameter, expression, TextRange.Unknown);
    }
}
