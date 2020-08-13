using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet.Contexts
{
    public readonly struct VariableInformation
    {
#if DEBUG
        public readonly string Symbol;
#endif
        public readonly IExpression SymbolHigherOrder;
        public readonly IExpression Expression;

        private VariableInformation(
            string symbol, IExpression symbolHigherOrder, IExpression expression)
        {
#if DEBUG
            this.Symbol = symbol;
#endif
            this.SymbolHigherOrder = symbolHigherOrder;
            this.Expression = expression;
        }

        public override string ToString() =>
#if DEBUG
            $"{this.Symbol}:{this.SymbolHigherOrder.GetPrettyString(PrettyStringTypes.Readable)} --> {this.Expression.GetPrettyString(PrettyStringTypes.Readable)}";
#else
            $"{this.SymbolHigherOrder.GetPrettyString(PrettyStringTypes.Readable)} --> {this.Expression.GetPrettyString(PrettyStringTypes.Readable)}";
#endif
        public static VariableInformation Create(
            string symbol, IExpression symbolHigherOrder, IExpression expression) =>
            new VariableInformation(symbol, symbolHigherOrder, expression);
    }

    public interface IScopeContext
    {
        ILogicalCalculator TypeCalculator { get; }

        VariableInformation[] LookupVariables(string symbol);

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

        private protected void MutableBind(IBoundVariableTerm symbol, IExpression expression)
        {
            this.variables ??= new Dictionary<string, List<VariableInformation>>();

            if (!this.variables.TryGetValue(symbol.Symbol, out var list))
            {
                list = new List<VariableInformation>();
                this.variables.Add(symbol.Symbol, list);
            }

            list.Add(
                VariableInformation.Create(
                    symbol.Symbol,
                    symbol.HigherOrder,
                    expression));
        }

        public VariableInformation[] LookupVariables(string symbol)
        {
            if (this.variables != null &&
                this.variables.TryGetValue(symbol, out var list))
            {
                return list.Memoize();
            }
            else
            {
                return
                    this.parent?.LookupVariables(symbol) ??
                    ArrayEx.Empty<VariableInformation>();
            }
        }
    }
}
