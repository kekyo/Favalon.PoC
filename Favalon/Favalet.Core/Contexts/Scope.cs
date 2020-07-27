using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Favalet.Contexts
{
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
    }
}
