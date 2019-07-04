using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Internals
{
    public static class StaticFactories
    {
        public static LiteralExpression Literal(object value, Expression higherOrder) =>
            new LiteralExpression(value, higherOrder);
        public static LiteralExpression Literal(object value) =>
            new LiteralExpression(value, UndefinedExpression.Instance);

        public static VariableExpression Variable(string name, Expression higherOrder) =>
            new VariableExpression(name, higherOrder);
        public static VariableExpression Variable(string name) =>
            new VariableExpression(name, UndefinedExpression.Instance);

        public static ApplyExpression Apply(Expression function, Expression parameter, Expression higherOrder) =>
            new ApplyExpression(function, parameter, higherOrder);
        public static ApplyExpression Apply(Expression function, Expression parameter) =>
            new ApplyExpression(function, parameter, UndefinedExpression.Instance);
    }
}
