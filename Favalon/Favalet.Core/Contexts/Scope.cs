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
        IReduceContext NewScope(IIdentityTerm parameter, IExpression expression);

        int NewPlaceholderIndex();

        IExpression InferHigherOrder(IExpression higherOrder);

        void Unify(IExpression fromHigherOrder, IExpression toHigherOrder);

        IExpression FixupHigherOrder(IExpression higherOrder);
    }

    public sealed class Scope : ScopeContext
    {
        private int placeholderIndex;

        private Scope(ILogicalCalculator typeCalculator) :
            base(null, typeCalculator)
        { }

        internal int DrawNewPlaceholderIndex() =>
            this.placeholderIndex++;

        public IExpression Infer(IExpression expression)
        {
            var context = new ReduceContext(
                this,
                this,
                new Dictionary<int, IExpression>());
            var inferred = expression.Infer(context);
            var fixupped = inferred.Fixup(context);

            return fixupped;
        }

        public IExpression Reduce(IExpression expression)
        {
            var context = new ReduceContext(
                this,
                this,
                new Dictionary<int, IExpression>());
            var reduced = expression.Reduce(context);

            return reduced;
        }

        public new void SetVariable(IIdentityTerm identity, IExpression expression) =>
            base.SetVariable(identity, expression);
        public void SetVariable(string identity, IExpression expression) =>
            base.SetVariable(IdentityTerm.Create(identity), expression);

        public static Scope Create(ILogicalCalculator typeCalculator) =>
            new Scope(typeCalculator);
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

            public int NewPlaceholderIndex() =>
                this.rootScope.DrawNewPlaceholderIndex();

            public IExpression InferHigherOrder(IExpression higherOrder)
            {
                var inferred = higherOrder.Infer(this);

                var context = new ReduceContext(
                    this.rootScope,
                    this,
                    this.unifiedExpressions);
                var reduced = inferred.Reduce(context);

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
                            this.unifiedExpressions[pto.Index] = from;
                            return;
                        }
                    }

                    // TODO: variance
                    this.unifiedExpressions[pfrom.Index] = to;
                    return;
                }
                else if (to is PlaceholderTerm pto)
                {
                    // TODO: variance
                    this.unifiedExpressions[pto.Index] = from;
                    return;
                }

                if (this.TypeCalculator.ExactEquals(from, to))
                {
                    return;
                }

                // Can't accept from --> to
                throw new ArgumentException();
            }

            public IExpression FixupHigherOrder(IExpression higherOrder)
            {
                return higherOrder;
            }

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
