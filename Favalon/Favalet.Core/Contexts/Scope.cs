using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Favalet.Contexts
{
    public interface IReduceContext :
        IScopeContext
    {
        IExpression Fixup(IExpression expression);

        IReduceContext NewScope(IIdentityTerm parameter, IExpression expression);

        int NewPlaceholderIndex();

        IExpression InferHigherOrder(IExpression higherOrder);

        void Unify(IExpression fromHigherOrder, IExpression toHigherOrder);

        IExpression? ResolvePlaceholderIndex(int index);
    }

    public sealed class Scope :
        ScopeContext, IScopeContext
    {
        private int placeholderIndex = -1;

        [DebuggerStepThrough]
        private Scope(ILogicalCalculator typeCalculator) :
            base(null, typeCalculator)
        { }

        [DebuggerStepThrough]
        internal int DrawNewPlaceholderIndex() =>
            Interlocked.Increment(ref this.placeholderIndex);

        public IExpression Infer(IExpression expression)
        {
            var context = new ReduceContext(
                this,
                this,
                new Dictionary<int, IExpression>());
            var inferred = context.Infer(expression);
            var fixupped = context.Fixup(inferred);

            return fixupped;
        }

        public IExpression Reduce(IExpression expression)
        {
            var context = new ReduceContext(
                this,
                this,
                new Dictionary<int, IExpression>());
            var reduced = context.Reduce(expression);

            return reduced;
        }

        [DebuggerHidden]
        public new void SetVariable(IIdentityTerm identity, IExpression expression) =>
            base.SetVariable(identity, expression);
        [DebuggerHidden]
        public void SetVariable(string identity, IExpression expression) =>
            base.SetVariable(IdentityTerm.Create(identity), expression);

        [DebuggerStepThrough]
        public static Scope Create(ILogicalCalculator typeCalculator) =>
            new Scope(typeCalculator);
        [DebuggerStepThrough]
        public static Scope Create() =>
            new Scope(LogicalCalculator.Instance);

        private sealed class ReduceContext :
            IReduceContext
        {
            private readonly Scope rootScope;
            private readonly IScopeContext parentScope;
            private readonly Dictionary<int, IExpression> unifiedExpressions;
            private IIdentityTerm? parameter;
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

            public IReduceContext NewScope(IIdentityTerm parameter, IExpression expression)
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
            public int NewPlaceholderIndex() =>
                this.rootScope.DrawNewPlaceholderIndex();

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

                if (from is PlaceholderTerm pfrom)
                {
                    if (to is PlaceholderTerm pto)
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
                                this.unifiedExpressions[pto.Index] = from;
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
                else if (to is PlaceholderTerm pto)
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

            public IExpression? ResolvePlaceholderIndex(int index) =>
                this.unifiedExpressions.TryGetValue(index, out var resolved) ?
                    resolved :
                    null;

            public IExpression? LookupVariable(IIdentityTerm identity) =>
                // TODO: improving when identity's higher order acceptable
                // TODO: what acceptable (narrowing, widening)
                this.parameter is IIdentityTerm p &&
                 expression is IExpression expr &&
                 p.Equals(identity) ?
                    expr :
                    parentScope.LookupVariable(identity);
        }
    }
}
