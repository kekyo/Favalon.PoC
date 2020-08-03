using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System.Diagnostics;
using System.Threading;

namespace Favalet
{
    public interface IEnvironments :
        IScopeContext
    {
        void MutableBind(IBoundVariableTerm symbol, IExpression expression);
    }

    public sealed class Environments :
        ScopeContext, IEnvironments, IPlaceholderProvider
    {
#if DEBUG
        private Unifier? lastUnifier;
#endif
        private int placeholderIndex = -1;

        [DebuggerStepThrough]
        private Environments(ILogicalCalculator typeCalculator) :
            base(null, typeCalculator)
        { }

        [DebuggerStepThrough]
        internal PlaceholderTerm CreatePlaceholder(
            PlaceholderOrderHints orderHint)
        {
            var ph = PlaceholderTerm.Create(
                this,
                Interlocked.Increment(ref this.placeholderIndex),
                orderHint);
#if DEBUG
            // Preassigns higher orders.
            var ho = ph.HigherOrder;
            while (!(ho is DeadEndTerm))
            {
                ho = ho.HigherOrder;
            }
#endif
            return ph;
        }

        [DebuggerStepThrough]
        public PlaceholderTerm CreatePlaceholder() =>
            this.CreatePlaceholder(PlaceholderOrderHints.VariableOrAbove);

        [DebuggerStepThrough]
        IIdentityTerm IPlaceholderProvider.CreatePlaceholder(
            PlaceholderOrderHints orderHint) =>
            this.CreatePlaceholder(orderHint);

        public IExpression Infer(IExpression expression)
        {
            var unifier = new Unifier(this.TypeCalculator);
            var context = new ReduceContext(this, this, unifier);

            Debug.WriteLine(
                $"Infer[{context.GetHashCode()}]: expression=\"{expression.GetXml()}\"");

            var inferred = context.Infer(expression);
#if DEBUG
            Debug.WriteLine(
                $"Infer[{context.GetHashCode()}]: inferred=\"{inferred.GetXml()}\", unifier=\"{unifier}\"");
#endif            
            var fixupped = context.Fixup(inferred);
            Debug.WriteLine(
                $"Infer[{context.GetHashCode()}]: fixupped=\"{fixupped.GetXml()}\"");
#if DEBUG
            this.lastUnifier = unifier;
#endif
            return fixupped;
        }

        public IExpression Reduce(IExpression expression)
        {
            var unifier = new Unifier(this.TypeCalculator);
            var context = new ReduceContext(this, this, unifier);

            Debug.WriteLine(
                $"Reduce[{context.GetHashCode()}]: expression=\"{expression.GetXml()}\"");

            var reduced = context.Reduce(expression);
            Debug.WriteLine(
                $"Reduce[{context.GetHashCode()}]: reduced=\"{reduced.GetXml()}\"");
#if DEBUG
            this.lastUnifier = unifier;
#endif
            return reduced;
        }

        [DebuggerStepThrough]
        public new void MutableBind(IBoundVariableTerm symbol, IExpression expression) =>
            base.MutableBind(symbol, expression);

        [DebuggerStepThrough]
        public static Environments Create(ILogicalCalculator typeCalculator)
        {
            var environment = new Environments(typeCalculator);
            return environment;
        }
    }

    public static class EnvironmentExtension
    {
        [DebuggerStepThrough]
        public static void MutableBind(
            this IEnvironments environment,
            string symbol,
            IExpression expression) =>
            environment.MutableBind(BoundVariableTerm.Create(symbol), expression);
    }
}
