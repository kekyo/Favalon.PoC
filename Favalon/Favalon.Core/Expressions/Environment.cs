using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class Environment
    {
        private Environment()
        { }

        public Expression Infer(Expression expression)
        {
            return expression;
        }

        public TExpression Infer<TExpression>(TExpression expression)
            where TExpression : Expression =>
            (TExpression)this.Infer((Expression)expression);

        public static Environment Create() =>
            new Environment();
    }
}
