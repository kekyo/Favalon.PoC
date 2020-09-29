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
        private ReduceContext? lastContext;
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

        private IExpression InternalInfer(
            ReduceContext context,
            IExpression expression)
        {
            Debug.WriteLine($"Infer[{context.GetHashCode()}:before] :");
            Debug.WriteLine(expression.GetXml());
  
            var rewritable = context.MakeRewritable(expression);
            context.SetTargetRoot(rewritable);

#if DEBUG
            Debug.WriteLine($"Infer[{context.GetHashCode()}:rewritable] :");
            Debug.WriteLine(expression.GetXml());
#endif            

            var inferred = context.Infer(rewritable);
            context.SetTargetRoot(inferred);
            
#if DEBUG
            Debug.WriteLine($"Infer[{context.GetHashCode()}:inferred] :");
            Debug.WriteLine(expression.GetXml());
#endif            

            var fixedup = context.Fixup(inferred);
            context.SetTargetRoot(fixedup);

#if DEBUG
            Debug.WriteLine($"Infer[{context.GetHashCode()}:fixedup] :");
            Debug.WriteLine(expression.GetXml());
#endif

            return fixedup;
        }

        public IExpression Infer(IExpression expression)
        {
            var unifier = Unifier.Create(this.TypeCalculator, expression);
            var context = new ReduceContext(this, this, unifier);

            var inferred = this.InternalInfer(context, expression);
            
#if DEBUG
            this.lastContext = context;
#endif

            return inferred;
        }

        public IExpression Reduce(IExpression expression)
        {
            var unifier = Unifier.Create(this.TypeCalculator, expression);
            var context = new ReduceContext(this, this, unifier);

            var inferred = this.InternalInfer(context, expression);
            var reduced = context.Reduce(inferred);
            
#if DEBUG
            Debug.WriteLine($"Reduce[{context.GetHashCode()}:reduced] :");
            Debug.WriteLine(reduced.GetXml());
            this.lastContext = context;
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
