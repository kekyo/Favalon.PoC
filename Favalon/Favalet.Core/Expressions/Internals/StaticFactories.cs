using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Internals
{
    public static class StaticFactories
    {
        public static UndefinedExpression Undefined() =>
            UndefinedExpression.Instance;

        public static KindExpression Kind() =>
            KindExpression.Instance;

        public static LiteralExpression Literal(object value, Expression higherOrder) =>
            new LiteralExpression(value, higherOrder);
        public static LiteralExpression Literal(object value) =>
            new LiteralExpression(value, UndefinedExpression.Instance);

        public static VariableExpression Variable(string name, Expression higherOrder) =>
            new VariableExpression(name, higherOrder);
        public static VariableExpression Variable(string name) =>
            new VariableExpression(name, UndefinedExpression.Instance);

        public static ApplyExpression Apply(Expression function, Expression argument, Expression higherOrder) =>
            new ApplyExpression(function, argument, higherOrder);
        public static ApplyExpression Apply(Expression function, Expression argument) =>
            new ApplyExpression(function, argument, UndefinedExpression.Instance);

        public static LambdaExpression Lambda(Expression parameter, Expression expression, Expression higherOrder) =>
            new LambdaExpression(parameter, expression, higherOrder);
        public static LambdaExpression Lambda(Expression parameter, Expression expression) =>
            new LambdaExpression(parameter, expression, UndefinedExpression.Instance);
    }
}
