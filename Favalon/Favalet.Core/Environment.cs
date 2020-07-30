using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
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
                new Unifier(this.TypeCalculator));

            Debug.WriteLine(
                $"Infer[{context.GetHashCode()}]: expression=\"{expression.GetPrettyString(PrettyStringContext.Strict)}\"");

            var inferred = context.Infer(expression);
            Debug.WriteLine(
                $"Infer[{context.GetHashCode()}]: inferred=\"{inferred.GetPrettyString(PrettyStringContext.Strict)}\"");
            
            var fixupped = context.Fixup(inferred);
            Debug.WriteLine(
                $"Infer[{context.GetHashCode()}]: fixupped=\"{fixupped.GetPrettyString(PrettyStringContext.Strict)}\"");

            return fixupped;
        }

        public IExpression Reduce(IExpression expression)
        {
            var context = new ReduceContext(
                this,
                this,
                new Unifier(this.TypeCalculator));

            Debug.WriteLine(
                $"Reduce[{context.GetHashCode()}]: expression=\"{expression.GetPrettyString(PrettyStringContext.Strict)}\"");

            var reduced = context.Reduce(expression);
            Debug.WriteLine(
                $"Reduce[{context.GetHashCode()}]: reduced=\"{reduced.GetPrettyString(PrettyStringContext.Strict)}\"");

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
