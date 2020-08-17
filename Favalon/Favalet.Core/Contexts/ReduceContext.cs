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
        IExpression MakeRewritableHigherOrder(
            IExpression higherOrder,
            bool replacePlaceholder = true);
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
        IExpression FixupHigherOrder(IExpression higherOrder);

        IExpression? Resolve(int index);
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
        private readonly ILogicalCalculator typeCalculator;

        [DebuggerStepThrough]
        protected FixupContext(ILogicalCalculator typeCalculator) =>
            this.typeCalculator = typeCalculator;

        [DebuggerStepThrough]
        public IExpression Fixup(IExpression expression) =>
            expression is Expression expr ? expr.InternalFixup(this) : expression;

        [DebuggerStepThrough]
        public IExpression FixupHigherOrder(IExpression higherOrder)
        {
            var fixupped = higherOrder is Expression expr ?
                expr.InternalFixup(this) :
                higherOrder;

            return this.typeCalculator.Compute(fixupped);
        }

        public abstract IExpression? Resolve(int index);
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
            Unifier unifier) :
            base(rootScope.TypeCalculator)
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
                original is IVariableTerm ||
                original is DeadEndTerm ||
                original is FourthTerm
                //||original is IFunctionExpression
                )
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

        [DebuggerStepThrough]
        public IExpression MakeRewritable(IExpression expression) =>
            expression is Expression expr ?
                expr.InternalMakeRewritable(this) :
                expression;

        public IExpression MakeRewritableHigherOrder(
            IExpression higherOrder,
            bool replacePlaceholder = true)
        {
            this.orderHint++;
            
            var rewritable = this.MakeRewritable(higherOrder);
            var result = replacePlaceholder ?
                this.CreatePlaceholderFrom(rewritable) :
                rewritable;
            
            this.orderHint--;
            Debug.Assert(this.orderHint >= PlaceholderOrderHints.VariableOrAbove);
            
            return result;
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
        public override IExpression? Resolve(int index) =>
            this.unifier.Resolve(index);

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
