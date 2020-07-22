using Favalet.Expressions;
using System;
using System.Collections.Generic;

namespace Favalet
{
    public interface IScopeContext
    {
        IExpression? LookupVariable(string symbol);
    }

    public abstract class ScopeContext :
        IScopeContext
    {
        private readonly ScopeContext? parent;
        private Dictionary<string, IExpression>? variables;

        internal ScopeContext(ScopeContext? parent) =>
            this.parent = parent;

        internal void SetVariable(string symbol, IExpression expression)
        {
            if (this.variables == null)
            {
                this.variables = new Dictionary<string, IExpression>();
            }
            this.variables[symbol] = expression;
        }

        public IExpression? LookupVariable(string symbol)
        {
            if (this.variables != null &&
                this.variables.TryGetValue(symbol, out var expression))
            {
                return expression;
            }
            else
            {
                return this.parent?.LookupVariable(symbol);
            }
        }
    }

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
                (this.symbol is string s &&
                 this.expression is IExpression expr &&
                 s.Equals(symbol)) ?
                    expr :
                    this.parent.LookupVariable(symbol);
        }
    }
}
