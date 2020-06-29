using System;

namespace Favalet
{
    public sealed class Scope
    {
        private Scope()
        { }

        public IExpression Reduce(IExpression expression)
        {
            var context = new ReduceContext();
            return expression.Reduce(context);
        }

        public static Scope Create() =>
            new Scope();

        private sealed class ReduceContext : IReduceContext
        {
        }
    }
}
