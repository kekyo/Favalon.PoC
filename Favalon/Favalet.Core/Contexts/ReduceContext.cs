using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet.Contexts
{
    public interface IReduceContext :
        IScopeContext
    {
        IExpression Fixup(IExpression expression);

        IReduceContext NewScope(IBoundSymbolTerm parameter, IExpression expression);

        IExpression InferHigherOrder(IExpression higherOrder);

        void Unify(IExpression fromHigherOrder, IExpression toHigherOrder);

        IExpression? ResolvePlaceholderIndex(int index);
    }

    internal sealed class ReduceContext :
        IReduceContext, IPlaceholderProvider
    {
        private readonly Scope rootScope;
        private readonly IScopeContext parentScope;
        private readonly Dictionary<int, IExpression> unifiedExpressions;
        private IBoundSymbolTerm? parameter;
        private IExpression? expression;

        [DebuggerStepThrough]
        public ReduceContext(
            Scope rootScope,
            IScopeContext parentScope,
            Dictionary<int, IExpression> unifiedExpressions)
        {
            this.rootScope = rootScope;
            this.parentScope = parentScope;
            this.unifiedExpressions = unifiedExpressions;
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

        public IReduceContext NewScope(
            IBoundSymbolTerm parameter, IExpression expression)
        {
            var newContext = new ReduceContext(
                this.rootScope,
                this,
                this.unifiedExpressions);

            newContext.parameter = parameter;
            newContext.expression = expression;

            return newContext;
        }

        [DebuggerStepThrough]
        public int AssignPlaceholderIndex() =>
            ((IPlaceholderProvider)this.rootScope).AssignPlaceholderIndex();

        public IExpression InferHigherOrder(IExpression higherOrder)
        {
            var inferred = this.Infer(higherOrder);

            var context = new ReduceContext(
                this.rootScope,
                this,
                this.unifiedExpressions);
            var reduced = context.Reduce(inferred);

            return reduced;
        }

        public void Unify(IExpression from, IExpression to)
        {
            Debug.Assert(!(from is UnspecifiedTerm));
            Debug.Assert(!(to is UnspecifiedTerm));

            if (from is IPlacehoderTerm pfrom)
            {
                if (to is IPlacehoderTerm pto)
                {
                    if (pfrom.Index == pto.Index)
                    {
                        return;
                    }
                    else
                    {
                        // TODO: variance
                        lock (this.unifiedExpressions)
                        {
                            if (this.unifiedExpressions.TryGetValue(pfrom.Index, out var target))
                            {
                                this.Unify(pto, target);
                            }
                            else
                            {
                                this.unifiedExpressions[pfrom.Index] = to;
                            }
                        }
                        return;
                    }
                }

                // TODO: variance
                lock (this.unifiedExpressions)
                {
                    this.unifiedExpressions[pfrom.Index] = to;
                }
                return;
            }
            else if (to is IPlacehoderTerm pto)
            {
                // TODO: variance
                lock (this.unifiedExpressions)
                {
                    this.unifiedExpressions[pto.Index] = from;
                }
                return;
            }

            if (this.TypeCalculator.ExactEquals(from, to))
            {
                return;
            }

            // Can't accept from --> to
            throw new ArgumentException();
        }

        public IExpression? ResolvePlaceholderIndex(int index)
        {
            var targetIndex = index;
            IExpression? lastExpression = null;
            while (true)
            {
                if (this.unifiedExpressions.TryGetValue(targetIndex, out var resolved))
                {
                    lastExpression = resolved;
                    if (lastExpression is IPlacehoderTerm placeholder)
                    {
                        targetIndex = placeholder.Index;
                        continue;
                    }
                }

                return lastExpression;
            }
        }

        public VariableInformation[] LookupVariables(IIdentityTerm identity) =>
            // TODO: improving when identity's higher order acceptable
            // TODO: what acceptable (narrowing, widening)
            this.parameter is IBoundSymbolTerm p &&
            expression is IExpression expr &&
            p.Symbol.Equals(identity.Symbol) ?
                new[] { VariableInformation.Create(p.HigherOrder, expr) } :
                parentScope.LookupVariables(identity);
    }
}
