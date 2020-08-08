using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Diagnostics;

namespace Favalet.Contexts
{
    public interface IReduceContext :
        IScopeContext, IPlaceholderProvider
    {
        IReduceContext Bind(IBoundVariableTerm parameter, IExpression expression);

        IExpression InferHigherOrder(IExpression higherOrder);
        IExpression Fixup(IExpression expression);

        void Unify(IExpression fromHigherOrder, IExpression toHigherOrder);

        IExpression? Resolve(string symbol);
    }

    internal sealed partial class ReduceContext :
        IReduceContext
    {
        private readonly Environments rootScope;
        private readonly IScopeContext parentScope;
        private readonly Unifier unifier;
        private IBoundVariableTerm? symbol;
        private IExpression? expression;

        [DebuggerStepThrough]
        public ReduceContext(
            Environments rootScope,
            IScopeContext parentScope,
            Unifier unifier)
        {
            this.rootScope = rootScope;
            this.parentScope = parentScope;
            this.unifier = unifier;
        }

        public ILogicalCalculator TypeCalculator =>
            this.rootScope.TypeCalculator;

        [DebuggerStepThrough]
        public IExpression Infer(IExpression expression) =>
            expression is Expression expr ? expr.InternalInfer(this) : expression;
        [DebuggerStepThrough]
        public IExpression Fixup(IExpression expression) =>
            expression is Expression expr ? expr.InternalFixup(this) : expression;
        [DebuggerStepThrough]
        public IExpression Reduce(IExpression expression) =>
            expression is Expression expr ? expr.InternalReduce(this) : expression;

        public IReduceContext Bind(
            IBoundVariableTerm symbol, IExpression expression)
        {
            var newContext = new ReduceContext(
                this.rootScope,
                this,
                this.unifier);

            newContext.symbol = symbol;
            newContext.expression = expression;

            return newContext;
        }

        [DebuggerStepThrough]
        public IIdentityTerm CreatePlaceholder(PlaceholderOrderHints orderHint) =>
            this.rootScope.CreatePlaceholder(orderHint);

        public IExpression InferHigherOrder(IExpression higherOrder)
        {
            var inferred = this.Infer(higherOrder);

            // Always replacing placeholder instead higher order.
            var placeholder =
                this.CreatePlaceholder(PlaceholderOrderHints.VariableOrAbove);

            this.Unify(placeholder, inferred);
            
            return placeholder;
        }

        [DebuggerStepThrough]
        public void Unify(IExpression fromHigherOrder, IExpression toHigherOrder) =>
            this.unifier.Unify(this, fromHigherOrder, toHigherOrder);

        [DebuggerStepThrough]
        public IExpression? Resolve(string symbol) =>
            this.unifier.Resolve(symbol);

        public VariableInformation[] LookupVariables(string symbol) =>
            // TODO: improving when identity's higher order acceptable
            // TODO: what acceptable (narrowing, widening)
            this.symbol is IBoundVariableTerm p &&
            expression is IExpression expr &&
            p.Symbol.Equals(symbol) ?
                new[] { VariableInformation.Create(symbol, p.HigherOrder, expr) } :
                parentScope.LookupVariables(symbol);

        public override string ToString() =>
            "ReduceContext: " + this.unifier;
    }
}
