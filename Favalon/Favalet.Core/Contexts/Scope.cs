using Favalet.Expressions;

namespace Favalet.Contexts
{
    public sealed class Scope : ScopeContext
    {
        private Scope() :
            base(null)
        { }

        public IExpression Reduce(IExpression expression)
        {
            var context = new ReduceContext(this);
            return expression.Reduce(context);
        }

        public new void SetVariable(string symbol, IExpression expression) =>
            base.SetVariable(symbol, expression);

        public static Scope Create() =>
            new Scope();

        private sealed class ReduceContext :
            IReduceContext
        {
            private readonly IScopeContext parent;
            private string? symbol;
            private IExpression? expression;

            public ReduceContext(IScopeContext parent) =>
                this.parent = parent;

            public IReduceContext NewScope(string symbol, IExpression expression)
            {
                var newContext = new ReduceContext(this);
                newContext.symbol = symbol;
                newContext.expression = expression;

                return newContext;
            }

            public IExpression? LookupVariable(string symbol) =>
                this.symbol is string s &&
                 expression is IExpression expr &&
                 s.Equals(symbol) ?
                    expr :
                    parent.LookupVariable(symbol);
        }
    }
}
