using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public static class Parser
    {
        public static Term Parse(string expressionString)
        {
            var expressions = expressionString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Term parse0(string expr)
            {
                if (int.TryParse(expr, out var number))
                {
                    return Factories.Number(number);
                }
                else
                {
                    return Factories.Variable(expr);
                }
            }

            // foldr
            return expressions.
                Reverse().
                Skip(1).
                Aggregate(parse0(expressions[expressions.Length - 1]), (ex1, ex0) => Factories.Apply(parse0(ex0), ex1));
        }
    }
}
