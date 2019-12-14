using Favalon.Contexts;
using System.Reflection;

namespace Favalon.Terms
{
    public sealed class ClrMethodTerm : HigherOrderLazyTerm, IApplicable
    {
        public new readonly MethodInfo Method;

        internal ClrMethodTerm(MethodInfo method) =>
            this.Method = method;

        protected override Term GetHigherOrder()
        {
            var parameters = this.Method.GetParameters();
            return LambdaTerm.Create(
                TypeTerm.From((parameters.Length == 0) ? typeof(void) : parameters[0].ParameterType),
                TypeTerm.From(this.Method.ReturnType));
        }

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        public Term? ReduceForApply(ReduceContext context, Term rhs)
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
}
