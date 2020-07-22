using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using System.Collections.Generic;
using System.Linq;

namespace Favalet.Contexts
{
    public abstract class ScopeContext :
        IScopeContext
    {
        private readonly ScopeContext? parent;
        private Dictionary<string, List<IExpression>>? variables;

        internal ScopeContext(ScopeContext? parent) =>
            this.parent = parent;

        internal void SetVariable(IIdentityTerm identity, IExpression expression)
        {
            if (this.variables == null)
            {
                this.variables = new Dictionary<string, List<IExpression>>();
            }
            if (!this.variables.TryGetValue(identity.Symbol, out var list))
            {
                list = new List<IExpression>();
                this.variables.Add(identity.Symbol, list);
            }
            list.Add(expression);
        }

        public IExpression? LookupVariable(IIdentityTerm identity)
        {
            if (this.variables != null &&
                this.variables.TryGetValue(identity.Symbol, out var list))
            {
                return list.Aggregate(OrExpression.Create);
            }
            else
            {
                return parent?.LookupVariable(identity);
            }
        }
    }

    public static class ScopeContextExtension
    {
        public static IExpression? LookupVariable(
            this IScopeContext context,
            string identity) =>
            context.LookupVariable(IdentityTerm.Create(identity));
    }
}
