﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public static class StaticFactories
    {
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
