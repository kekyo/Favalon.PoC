using System;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Diagnostics;
using System.Reflection;

namespace Favalet.Contexts
{
    public interface IMakeRewritableContext
    {
        IExpression CreatePlaceholderFrom(IExpression original);
        
        IExpression MakeRewritable(IExpression expression);
        IExpression MakeRewritableHigherOrder(IExpression higherOrder);
    }
    
    public interface IInferContext :
        IScopeContext, IMakeRewritableContext
    {
        IExpression Infer(IExpression expression);
    
        IInferContext Bind(IBoundVariableTerm parameter, IExpression expression);

        void Unify(IExpression fromHigherOrder, IExpression toHigherOrder);
    }

    public interface IFixupContext
    {
        IExpression Fixup(IExpression expression);

        IExpression? Resolve(string symbol);
    }

    public interface IReduceContext :
        IScopeContext
    {
        IExpression Reduce(IExpression expression);
    
        IReduceContext Bind(IBoundVariableTerm parameter, IExpression expression);
    }

    internal abstract class FixupContext :
        IFixupContext
    {
        [DebuggerStepThrough]
        protected FixupContext()
        {
        }

        [DebuggerStepThrough]
        public IExpression Fixup(IExpression expression) =>
            expression is Expression expr ? expr.InternalFixup(this) : expression;

        public abstract IExpression? Resolve(string symbol);
    }

    internal sealed partial class ReduceContext :
        FixupContext, IInferContext, IReduceContext
    {
        private readonly Environments rootScope;
        private readonly IScopeContext parentScope;
        private readonly Unifier unifier;
        private IBoundVariableTerm? boundSymbol;
        private IExpression? boundExpression;
        private PlaceholderOrderHints orderHint = PlaceholderOrderHints.VariableOrAbove;

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

        public IExpression CreatePlaceholderFrom(IExpression original)
        {
            if (this.orderHint >= PlaceholderOrderHints.DeadEnd)
            {
                return DeadEndTerm.Instance;
            }
            
            // Cannot replace these terms.
            if (original is IPlaceholderTerm ||
                original is DeadEndTerm ||
                original is FourthTerm ||
                original is IFunctionExpression)
            {
                return original;
            }
            
            var placeholder = this.rootScope.CreatePlaceholder(this.orderHint);
            
            // The placeholder will be independent by a unspecified term.
            if (!(original is UnspecifiedTerm))
            {
                this.unifier.Unify(this, placeholder, original);
            }
            
            return placeholder;
        }

        public IExpression MakeRewritable(IExpression expression) =>
            expression is Expression expr ?
                expr.InternalMakeRewritable(this) :
                expression;

        public IExpression MakeRewritableHigherOrder(IExpression higherOrder)
        {
            this.orderHint++;
            
            var rewritable = this.MakeRewritable(higherOrder);
            var placeholder = this.CreatePlaceholderFrom(rewritable);
            
            this.orderHint--;
            Debug.Assert(this.orderHint >= PlaceholderOrderHints.VariableOrAbove);
            
            return placeholder;
        }

        [DebuggerStepThrough]
        public IExpression Infer(IExpression expression) =>
            expression is Expression expr ? expr.InternalInfer(this) : expression;
        [DebuggerStepThrough]
        public IExpression Reduce(IExpression expression) =>
            expression is Expression expr ? expr.InternalReduce(this) : expression;

        private ReduceContext Bind(
            IBoundVariableTerm symbol, IExpression expression)
        {
            var newContext = new ReduceContext(
                this.rootScope,
                this,
                this.unifier);

            newContext.boundSymbol = symbol;
            newContext.boundExpression = expression;

            return newContext;
        }

        [DebuggerStepThrough]
        IInferContext IInferContext.Bind(
            IBoundVariableTerm symbol, IExpression expression) =>
            this.Bind(symbol, expression);
        [DebuggerStepThrough]
        IReduceContext IReduceContext.Bind(
            IBoundVariableTerm symbol, IExpression expression) =>
            this.Bind(symbol, expression);

        [DebuggerStepThrough]
        public void Unify(IExpression fromHigherOrder, IExpression toHigherOrder) =>
            this.unifier.Unify(this, fromHigherOrder, toHigherOrder);

        [DebuggerStepThrough]
        public override IExpression? Resolve(string symbol) =>
            this.unifier.Resolve(symbol);

        public VariableInformation[] LookupVariables(string symbol) =>
            // TODO: improving when identity's higher order acceptable
            // TODO: what acceptable (narrowing, widening)
            this.boundSymbol is IBoundVariableTerm p &&
            boundExpression is IExpression expr &&
            p.Symbol.Equals(symbol) ?
                new[] { VariableInformation.Create(symbol, p.HigherOrder, expr) } :
                parentScope.LookupVariables(symbol);

        public override string ToString() =>
            "ReduceContext: " + this.unifier;
    }
}
