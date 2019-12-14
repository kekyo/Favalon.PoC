using Favalon.Contexts;
using Favalon.Terms.AlgebricData;
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
                new ClrMethodOverloadedTerm(ms);
        }
    }

    public sealed class ClrMethodTerm : MethodTerm, IApplicable
    {
        public new readonly MethodInfo Method;

        internal ClrMethodTerm(MethodInfo method) =>
            this.Method = method;

        protected override Term GetHigherOrder() =>
            GetMethodHigherOrder(this.Method);

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs)
        {
            var reduced = rhs.Reduce(context);

            if (reduced is ConstantTerm constant)
            {
                return new ConstantTerm(
                    this.Method.Invoke(null, new object[] { constant.Value }));
            }
            else
            {
                return null;
            }
        }

        public override bool Equals(Term? other) =>
            other is ClrMethodTerm rhs ?
                this.Method.Equals(rhs.Method) :
                false;

        public override int GetHashCode() =>
            this.Method.GetHashCode();
    }

    public sealed class ClrMethodOverloadedTerm : MethodTerm
    {
        private static readonly int hashCode =
            typeof(ClrMethodOverloadedTerm).GetHashCode();

        public readonly MethodInfo[] Methods;

        internal ClrMethodOverloadedTerm(MethodInfo[] methods) =>
            this.Methods = methods;

        protected override Term GetHigherOrder() =>
            new OrTerm(this.Methods.Select(GetMethodHigherOrder).Distinct().ToArray());

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is ClrMethodOverloadedTerm rhs ?
                this.Methods.SequenceEqual(rhs.Methods) :
                false;

        public override int GetHashCode() =>
            this.Methods.Aggregate(hashCode, (v, method) => v ^ method.GetHashCode());
    }
}
