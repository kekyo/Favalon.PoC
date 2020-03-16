using Favalon.Terms.Contexts;
using Favalon.Terms.Types;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms.Methods
{
    public sealed class ClrMethodTerm : Term, IValueTerm, IApplicableTerm
    {
        private readonly MethodInfo method;

        private ClrMethodTerm(MethodInfo method) =>
            this.method = method;

        public override Term HigherOrder
        {
            get
            {
                var parameters = method.GetParameters();
                var returnTerm = ClrTypeTerm.From(method.ReturnType);
                return parameters.Length switch
                {
                    0 => LambdaTerm.From(ClrTypeTerm.Void, returnTerm),
                    _ => (LambdaTerm)parameters.Reverse().Aggregate(
                        returnTerm,
                        (term, p) => LambdaTerm.From(ClrTypeTerm.From(p.ParameterType), term)),
                };
            }
        }

        object IValueTerm.Value =>
            this.method;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        internal Term Invoke(IValueTerm argument) =>
            ClrConstantTerm.From(this.method.Invoke(null, new object[] { argument.Value }));

        AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term appliedHigherOrderHint) =>
            argument.Reduce(context) is Term argumentTerm ?
                (argumentTerm is IValueTerm valueTerm ?
                    AppliedResult.Applied(this.Invoke(valueTerm), argumentTerm) :
                    AppliedResult.Ignored(this, argumentTerm)) :
                AppliedResult.Ignored(this, argument);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ClrMethodTerm rhs ?
                this.method.Equals(rhs.method) :
                false;

        public override int GetHashCode() =>
            this.method.GetHashCode();

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.method.DeclaringType.PrettyPrint(context)}.{this.method.Name}";

        public static Term From(MethodInfo method) =>
            new ClrMethodTerm(method);   // TODO: generic method
    }
}
