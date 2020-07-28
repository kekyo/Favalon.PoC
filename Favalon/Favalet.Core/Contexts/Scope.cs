using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Favalet.Contexts
{
    public sealed class Scope :
        ScopeContext, IScopeContext, IPlaceholderProvider
    {
        private int placeholderIndex = -1;

        [DebuggerStepThrough]
        private Scope(ILogicalCalculator typeCalculator) :
            base(null, typeCalculator)
        { }

        [DebuggerStepThrough]
        private int AssignPlaceholderIndex() =>
            Interlocked.Increment(ref this.placeholderIndex);
        [DebuggerStepThrough]
        int IPlaceholderProvider.AssignPlaceholderIndex() =>
            this.AssignPlaceholderIndex();

        [DebuggerStepThrough]
        public PlaceholderTerm CreatePlaceholder(
            PlaceholderOrders candidateOrder = PlaceholderOrders.Type) =>
            PlaceholderTerm.Create(
                this,
                this.AssignPlaceholderIndex(),
                candidateOrder);

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
        public new void SetVariable(IBoundSymbolTerm symbol, IExpression expression) =>
            base.SetVariable(symbol, expression);
        [DebuggerHidden]
        public void SetVariable(string symbol, IExpression expression) =>
            base.SetVariable(BoundSymbolTerm.Create(symbol), expression);

        [DebuggerStepThrough]
        public static Scope Create(ILogicalCalculator typeCalculator) =>
            new Scope(typeCalculator);
        [DebuggerStepThrough]
        public static Scope Create() =>
            new Scope(LogicalCalculator.Instance);
    }
}
