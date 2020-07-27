using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Contexts
{
    public interface IScopeContext
    {
        ILogicalCalculator TypeCalculator { get; }

        IExpression? LookupVariable(IIdentityTerm identity);

        IExpression Infer(IExpression expression);
        IExpression Reduce(IExpression expression);
    }

    public abstract class ScopeContext
    {
        private readonly ScopeContext? parent;
        private Dictionary<string, List<IExpression>>? variables;

        [DebuggerStepThrough]
        internal ScopeContext(ScopeContext? parent, ILogicalCalculator typeCalculator)
        {
            this.parent = parent;
            this.TypeCalculator = typeCalculator;
        }

        public ILogicalCalculator TypeCalculator { get; }

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
