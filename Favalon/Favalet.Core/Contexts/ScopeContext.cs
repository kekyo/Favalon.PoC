using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Contexts
{
    public struct VariableInformation
    {
        public readonly IExpression SymbolHigherOrder;
        public readonly IExpression Expression;

        private VariableInformation(
            IExpression symbolHigherOrder, IExpression expression)
        {
            this.SymbolHigherOrder = symbolHigherOrder;
            this.Expression = expression;
        }

        public static VariableInformation Create(
            IExpression symbolHigherOrder, IExpression expression) =>
            new VariableInformation(symbolHigherOrder, expression);
    }

    public interface IScopeContext
    {
        ILogicalCalculator TypeCalculator { get; }

        VariableInformation[] LookupVariables(IIdentityTerm identity);

        IExpression Infer(IExpression expression);
        IExpression Reduce(IExpression expression);
    }

    public abstract class ScopeContext
    {
        private readonly ScopeContext? parent;
        private Dictionary<string, List<VariableInformation>>? variables;

        [DebuggerStepThrough]
        internal ScopeContext(ScopeContext? parent, ILogicalCalculator typeCalculator)
        {
            this.parent = parent;
            this.TypeCalculator = typeCalculator;
        }

        public ILogicalCalculator TypeCalculator { get; }

        private protected void MutableBind(IBoundSymbolTerm symbol, IExpression expression)
        {
            if (this.variables == null)
            {
                this.variables = new Dictionary<string, List<VariableInformation>>();
            }

            if (!this.variables.TryGetValue(symbol.Symbol, out var list))
            {
                list = new List<VariableInformation>();
                this.variables.Add(symbol.Symbol, list);
            }

            list.Add(VariableInformation.Create(symbol.HigherOrder, expression));
        }

        public VariableInformation[] LookupVariables(IIdentityTerm identity)
        {
            if (this.variables != null &&
                this.variables.TryGetValue(identity.Symbol, out var list))
            {
                return list.ToArray();
            }
            else
            {
                return
                    this.parent?.LookupVariables(identity) ??
                    ArrayEx.Empty<VariableInformation>();
            }
        }
    }

    public static class ScopeContextExtension
    {
        public static VariableInformation[] LookupVariables(
            this IScopeContext context,
            string identity) =>
            context.LookupVariables(IdentityTerm.Create(identity));
    }
}
