using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Diagnostics;

namespace Favalet.Contexts
{
    public interface IReduceContext :
        IScopeContext, IPlaceholderProvider
    {
        IExpression Fixup(IExpression expression);

        IReduceContext Bind(IBoundSymbolTerm parameter, IExpression expression);

        IExpression InferHigherOrder(IExpression higherOrder);

        void Unify(IExpression fromHigherOrder, IExpression toHigherOrder);

        IExpression? ResolvePlaceholderIndex(int index);
    }

    internal sealed partial class ReduceContext :
        IReduceContext
    {
        private readonly Environments rootScope;
        private readonly IScopeContext parentScope;
        private readonly Unifier unifier;
        private IBoundSymbolTerm? symbol;
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

        [DebuggerHidden]
        public IExpression Infer(IExpression expression) =>
            expression is Expression expr ? expr.InternalInfer(this) : expression;
        [DebuggerHidden]
        public IExpression Fixup(IExpression expression) =>
            expression is Expression expr ? expr.InternalFixup(this) : expression;
        [DebuggerHidden]
        public IExpression Reduce(IExpression expression) =>
            expression is Expression expr ? expr.InternalReduce(this) : expression;

        public IReduceContext Bind(
            IBoundSymbolTerm symbol, IExpression expression)
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
        public IPlaceholderTerm CreatePlaceholder(PlaceholderOrderHints candidateOrder) =>
            this.rootScope.CreatePlaceholder(candidateOrder);

        public IExpression InferHigherOrder(IExpression higherOrder)
        {
            var inferred = this.Infer(higherOrder);
            var fixupped = this.Fixup(inferred);

            var context = new ReduceContext(
                this.rootScope,
                this,
                this.unifier);
            var reduced = context.Reduce(fixupped);

            return reduced;
        }

        [DebuggerHidden]
        public void Unify(IExpression fromHigherOrder, IExpression toHigherOrder) =>
            this.unifier.Unify(fromHigherOrder, toHigherOrder);

        [DebuggerHidden]
        public IExpression? ResolvePlaceholderIndex(int index) =>
            this.unifier.ResolvePlaceholderIndex(index);

        public VariableInformation[] LookupVariables(IIdentityTerm identity) =>
            // TODO: improving when identity's higher order acceptable
            // TODO: what acceptable (narrowing, widening)
            this.symbol is IBoundSymbolTerm p &&
            expression is IExpression expr &&
            p.Symbol.Equals(identity.Symbol) ?
                new[] { VariableInformation.Create(p.HigherOrder, expr) } :
                parentScope.LookupVariables(identity);
    }
}
