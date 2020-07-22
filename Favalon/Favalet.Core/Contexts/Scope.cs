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

        public new void SetVariable(IIdentityTerm identity, IExpression expression) =>
            base.SetVariable(identity, expression);
        public void SetVariable(string identity, IExpression expression) =>
            base.SetVariable(IdentityTerm.Create(identity), expression);

        public static Scope Create() =>
            new Scope();

        private sealed class ReduceContext :
            IReduceContext
        {
            private readonly IScopeContext parent;
            private IIdentityTerm? parameter;
            private IExpression? expression;

            public ReduceContext(IScopeContext parent) =>
                this.parent = parent;

            public IReduceContext NewScope(IIdentityTerm parameter, IExpression expression)
            {
                var newContext = new ReduceContext(this);
                newContext.parameter = parameter;
                newContext.expression = expression;

                return newContext;
            }

            public IExpression? LookupVariable(IIdentityTerm identity) =>
                // TODO: improving when identity's higher order acceptable
                // TODO: what acceptable (narrowing, widening)
                this.parameter is IIdentityTerm p &&
                 expression is IExpression expr &&
                 p.Equals(identity) ?
                    expr :
                    parent.LookupVariable(identity);
        }
    }
}
