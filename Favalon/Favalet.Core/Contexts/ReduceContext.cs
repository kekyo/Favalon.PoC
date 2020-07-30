using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

    internal sealed class ReduceContext :
        IReduceContext
    {
        private readonly Environment rootScope;
        private readonly IScopeContext parentScope;
        private readonly Dictionary<int, IExpression> unifiedExpressions;
        private IBoundSymbolTerm? symbol;
        private IExpression? expression;

        [DebuggerStepThrough]
        public ReduceContext(
            Environment rootScope,
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

        public IReduceContext Bind(
            IBoundSymbolTerm symbol, IExpression expression)
        {
            var newContext = new ReduceContext(
                this.rootScope,
                this,
                this.unifiedExpressions);

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
                this.unifiedExpressions);
            var reduced = context.Reduce(fixupped);

            return reduced;
        }

        private void InternalUnify(IExpression from, IExpression to)
        {
            Debug.Assert(!(from is UnspecifiedTerm));
            Debug.Assert(!(to is UnspecifiedTerm));

            if (from is IPlaceholderTerm(int findex))
            {
                if (to is IPlaceholderTerm(int tindex))
                {
                    if (findex != tindex)
                    {
                        // TODO: variance
                        lock (this.unifiedExpressions)
                        {
                            if (this.unifiedExpressions.TryGetValue(findex, out var target))
                            {
                                this.Unify(to, target);
                            }
                            else
                            {
                                this.unifiedExpressions[findex] = to;
                            }
                        }
                    }
                }
                else
                {
                    // TODO: variance
                    lock (this.unifiedExpressions)
                    {
                        if (this.unifiedExpressions.TryGetValue(findex, out var target))
                        {
                            this.Unify(to, target);
                        }
                        else
                        {
                            this.unifiedExpressions[findex] = to;
                        }
                    }
                }
                return;
            }
            else if (to is IPlaceholderTerm(int tindex))
            {
                // TODO: variance
                lock (this.unifiedExpressions)
                {
                    if (this.unifiedExpressions.TryGetValue(tindex, out var target))
                    {
                        this.Unify(from, target);
                    }
                    else
                    {
                        this.unifiedExpressions[tindex] = from;
                    }
                }
                return;
            }

            if (from is IFunctionExpression(IExpression fp, IExpression fr) &&
                to is IFunctionExpression(IExpression tp, IExpression tr))
            {
                this.Unify(fp, tp);
                this.Unify(fr, tr);
                return;
            }

            if (this.TypeCalculator.ExactEquals(from, to))
            {
                return;
            }

            // Can't accept from --> to
            throw new ArgumentException(
                $"Couldn't accept unification: From=\"{from.GetPrettyString(PrettyStringContext.Simple)}\", To=\"{to.GetPrettyString(PrettyStringContext.Simple)}\".");
        }

        public void Unify(IExpression from, IExpression to)
        {
            this.InternalUnify(from, to);

            switch (from, to)
            {
                case (FourthTerm _, _):
                case (_, FourthTerm _):
                    break;

                default:
                    this.Unify(from.HigherOrder, to.HigherOrder);
                    break;
            }
        }

        public IExpression? ResolvePlaceholderIndex(int index)
        {
            var taken = new HashSet<int>();
            var list = new List<int>();
            var targetIndex = index;
            IExpression? lastExpression = null;

            while (true)
            {
                list.Add(targetIndex);

                if (taken.Add(targetIndex))
                {
                    if (this.unifiedExpressions.TryGetValue(targetIndex, out var resolved))
                    {
                        lastExpression = resolved;
                        if (lastExpression is IPlaceholderTerm placeholder)
                        {
                            targetIndex = placeholder.Index;
                            continue;
                        }
                    }

                    return lastExpression;
                }

                throw new InvalidOperationException(
                    "Detected circular variable reference: " +
                    StringUtilities.Join(" --> ", list.Select(index => $"'{index}")));
            }
        }

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
