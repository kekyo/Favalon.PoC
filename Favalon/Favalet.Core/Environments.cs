using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Expressions.Specialized;
using Favalet.Contexts.Unifiers;
using System;
using System.Diagnostics;
using System.Linq;
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
        private Environments(ITypeCalculator typeCalculator) :
            base(null, typeCalculator)
        {
            this.MutableBind(Generator.kind.Symbol, Generator.kind);
        }

        [DebuggerStepThrough]
        internal IExpression CreatePlaceholder(
            PlaceholderOrderHints orderHint)
        {
            var oh = Math.Min(
                (int)PlaceholderOrderHints.DeadEnd,
                Math.Max(0, (int)orderHint));
            var count = Math.Min(
                (int)PlaceholderOrderHints.DeadEnd - oh,
                (int)PlaceholderOrderHints.Fourth);
            
            var indexList =
                Enumerable.Range(0, count).
                Select(_ => Interlocked.Increment(ref this.placeholderIndex)).
                ToArray();
            
            return indexList.
                Reverse().
                Aggregate(
                    (IExpression)DeadEndTerm.Instance,
                    (agg, index) => PlaceholderTerm.Create(index, agg));
        }

        [DebuggerStepThrough]
        IExpression IPlaceholderProvider.CreatePlaceholder(
            PlaceholderOrderHints orderHint) =>
            this.CreatePlaceholder(orderHint);

        public IExpression Infer(IExpression expression)
        {
            var unifier = Unifier.Create(this.TypeCalculator, expression);
            var context = new ReduceContext(this, this, unifier);

            Debug.WriteLine(
                $"Infer[{context.GetHashCode()}]: expression=\"{expression.GetXml()}\"");

            var rewritable = context.MakeRewritable(expression);
            unifier.SetTargetRoot(rewritable);

#if DEBUG
            Debug.WriteLine(
                $"Infer[{context.GetHashCode()}]: rewritable=\"{rewritable.GetXml()}\", unifier=\"{unifier}\"");
#endif            

            var inferred = context.Infer(rewritable);
            unifier.SetTargetRoot(inferred);

            var dot = unifier.Dot;
            
#if DEBUG
            Debug.WriteLine(
                $"Infer[{context.GetHashCode()}]: inferred=\"{inferred.GetXml()}\", unifier=\"{unifier}\"");
#endif            

            var fixedup = context.Fixup(inferred);
            unifier.SetTargetRoot(fixedup);

#if DEBUG
            Debug.WriteLine(
                $"Infer[{context.GetHashCode()}]: fixedup=\"{fixedup.GetXml()}\"");
            this.lastUnifier = unifier;
#endif
            return fixedup;
        }

        public IExpression Reduce(IExpression expression)
        {
            var unifier = Unifier.Create(this.TypeCalculator, expression);
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
        public static Environments Create(ITypeCalculator typeCalculator)
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
