using Favalon.Contexts;
using Favalon.Terms.Algebric;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms
{
    public abstract class MethodTerm : HigherOrderLazyTerm
    {
        private protected MethodTerm()
        { }

        private protected static LambdaTerm GetMethodHigherOrder(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return LambdaTerm.Create(
                TypeTerm.From((parameters.Length == 0) ? typeof(void) : parameters[0].ParameterType),
                TypeTerm.From(method.ReturnType));
        }

        public static MethodTerm From(IEnumerable<MethodInfo> methods)
        {
            var ms = methods.ToArray();
            return (ms.Length == 1) ?
                (MethodTerm)new ClrMethodTerm(ms[0]) :
                new ClrMethodOverloadedTerm(ms.Select(method => new ClrMethodTerm(method)).ToArray());
        }
    }

    public sealed class ClrMethodTerm : MethodTerm, IApplicable
    {
        private readonly MethodInfo method;

        internal ClrMethodTerm(MethodInfo method) =>
            this.method = method;

        protected override Term GetHigherOrder() =>
            GetMethodHigherOrder(this.method);

        public Term ParameterHigherOrder =>
            ((LambdaTerm)this.HigherOrder).Parameter;

        public Term ReturnHigherOrder =>
            ((LambdaTerm)this.HigherOrder).Body;

        public override Term Infer(InferContext context) =>
            this;

        Term IApplicable.InferForApply(InferContext context, Term rhs)
        {
            var argument = rhs.Infer(context);

            context.Unify(
                ((LambdaTerm)this.HigherOrder).Parameter,
                argument.HigherOrder);

            return this;
        }

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            (rhs.Reduce(context) is ConstantTerm constant &&
             TypeTerm.IsAssignableFrom(constant.HigherOrder, ((LambdaTerm)this.HigherOrder).Parameter)) ?
                new ConstantTerm(this.method.Invoke(null, new object[] { constant.Value })) :
                null;

        public override bool Equals(Term? other) =>
            other is ClrMethodTerm rhs ?
                this.method.Equals(rhs.method) :
                false;

        public override int GetHashCode() =>
            this.method.GetHashCode();
    }

    public sealed class ClrMethodOverloadedTerm : MethodTerm, IApplicable
    {
        private static readonly int hashCode =
            typeof(ClrMethodOverloadedTerm).GetHashCode();

        public readonly ClrMethodTerm[] Methods;

        internal ClrMethodOverloadedTerm(ClrMethodTerm[] methods) =>
            this.Methods = methods;

        protected override Term GetHigherOrder() =>
            new SumTerm(this.Methods.
                Select(method => method.HigherOrder).
                Distinct().
                ToArray());

        public override Term Infer(InferContext context) =>
            // Best effort infer procedure: cannot fixed.
            this;

        Term IApplicable.InferForApply(InferContext context, Term rhs)
        {
            // Strict infer procedure.

            var argument = rhs.Infer(context);

            var selectedMethods = this.Methods.
                Where(method => TypeTerm.IsAssignableFrom(method.ParameterHigherOrder, argument.HigherOrder)).
                ToArray();

            return selectedMethods.Length switch
            {
                // Exact matched.
                1 => selectedMethods[0],
                _ => (selectedMethods.Length != this.Methods.Length) ?
                    // Partially matched.
                    new ClrMethodOverloadedTerm(selectedMethods) :
                    // All matched: not changed.
                    this
            };
        }

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            // Cannot apply because it isn't fixed overloads.
            null;

        public override bool Equals(Term? other) =>
            other is ClrMethodOverloadedTerm rhs ?
                this.Methods.SequenceEqual(rhs.Methods) :
                false;

        public override int GetHashCode() =>
            this.Methods.Aggregate(hashCode, (v, method) => v ^ method.GetHashCode());
    }
}
