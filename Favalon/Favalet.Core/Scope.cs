using System;

namespace Favalet
{
    public sealed class Scope
    {
        private Scope()
        { }

        public IExpression Reduce(IExpression expression)
        {
            return expression;
        }

        public static Scope Create() =>
            new Scope();
    }
}
