using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions.Internals
{
    public static class StaticFactory
    {
        public static VariableExpression Variable(string name) =>
            new VariableExpression(name, UndefinedExpression.Instance);
        public static VariableExpression Variable(string name, TermExpression higherOrder) =>
            new VariableExpression(name, higherOrder);

        public static ApplyExpression Apply(TermExpression function, TermExpression parameter) =>
            new ApplyExpression(function, parameter,
                (function is LambdaExpression lambda) ? lambda.Expression.HigherOrder : UndefinedExpression.Instance);

        public static LambdaExpression Lambda(TermExpression parameter, TermExpression expression) =>
            new LambdaExpression(parameter, expression, new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, KindExpression.Instance));

        public static ConstantExpression Constant(object value) =>
            ConstantExpression.Create(value);

        public static VariableExpression Type(string typeName) =>
            new VariableExpression(typeName, KindExpression.Instance);

        public static BindExpression Bind(VariableExpression bound, TermExpression expression, TermExpression body) =>
            new BindExpression(bound, expression, body, body.HigherOrder);
    }
}
