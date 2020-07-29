using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Favalet
{
    public interface IEnvironment :
        IScopeContext, IPlaceholderProvider
    {
        void MutableBind(IBoundSymbolTerm symbol, IExpression expression);
    }

    public sealed class Environment :
        ScopeContext, IEnvironment
    {
        private int placeholderIndex = -1;

        [DebuggerStepThrough]
        private Environment(ILogicalCalculator typeCalculator) :
            base(null, typeCalculator)
        { }

        [DebuggerStepThrough]
        public PlaceholderTerm CreatePlaceholder(
            PlaceholderOrderHints orderHint = PlaceholderOrderHints.TypeOrAbove) =>
            PlaceholderTerm.Create(
                this,
                Interlocked.Increment(ref this.placeholderIndex),
                orderHint);
        [DebuggerStepThrough]
        IPlaceholderTerm IPlaceholderProvider.CreatePlaceholder(
            PlaceholderOrderHints orderHint) =>
            this.CreatePlaceholder(orderHint);

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
        public new void MutableBind(IBoundSymbolTerm symbol, IExpression expression) =>
            base.MutableBind(symbol, expression);

        [DebuggerStepThrough]
        public static Environment Create(ILogicalCalculator typeCalculator)
        {
            var environment = new Environment(typeCalculator);
            return environment;
        }
    }

    public static class EnvironmentExtension
    {
        [DebuggerHidden]
        public static void MutableBind(
            this IEnvironment environment,
            string symbol,
            IExpression expression) =>
            environment.MutableBind(BoundSymbolTerm.Create(symbol), expression);
    }
}
