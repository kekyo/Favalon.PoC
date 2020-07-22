using Favalet.Expressions;
using System;
using System.Collections.Generic;

namespace Favalet.Contexts
{
    public abstract class ScopeContext :
        IScopeContext
    {
        private readonly ScopeContext? parent;
        private Dictionary<string, IExpression>? variables;

        internal ScopeContext(ScopeContext? parent) =>
            this.parent = parent;

        internal void SetVariable(string symbol, IExpression expression)
        {
            if (variables == null)
            {
                variables = new Dictionary<string, IExpression>();
            }
            variables[symbol] = expression;
        }

        public IExpression? LookupVariable(string symbol)
        {
            if (variables != null &&
                variables.TryGetValue(symbol, out var expression))
            {
                return expression;
            }
            else
            {
                return parent?.LookupVariable(symbol);
            }
        }
    }
}
