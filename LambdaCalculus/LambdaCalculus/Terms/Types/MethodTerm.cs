using Favalon.Contexts;
using Favalon.Terms.Algebric;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms.Types
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

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument)
        {
            context.Unify(
                ((LambdaTerm)this.HigherOrder).Parameter,
                inferredArgument.HigherOrder);

            return this;
        }

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument) =>
            (argument.Reduce(context) is ConstantTerm constant &&
             TypeTerm.Narrow(((LambdaTerm)this.HigherOrder).Parameter, constant.HigherOrder) is ClrTypeTerm) ?
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

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument)
        {
            // Strict infer procedure.

            var narrowed = this.Methods.
                Select(method => (
                    method,
                    narrow: TypeTerm.Narrow(method.ParameterHigherOrder, inferredArgument.HigherOrder))).
                Where(entry => entry.narrow is ClrTypeTerm).
                Select(entry => (entry.method, narrow: (ClrTypeTerm)entry.narrow!)).
                OrderBy(entry => entry.narrow, TypeTerm.ConcreterComparer).
                ToArray();

            var exactMatched = narrowed.
                Where(entry => entry.narrow.Equals(inferredArgument.HigherOrder)).
                ToArray();

            return exactMatched.Length switch
            {
                // Exact matched.
                1 => exactMatched[0].method,
                _ => (narrowed.Length != this.Methods.Length) ?
                    // Partially matched.
                    new ClrMethodOverloadedTerm(narrowed.Select(entry => entry.method).ToArray()) :
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
