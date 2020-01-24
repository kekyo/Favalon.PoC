using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;
using Favalon.Terms.Types;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms.Methods
{
    public sealed class ClrMethodTerm : Term, IValueTerm, IApplicableTerm
    {
        private readonly MethodInfo method;

        private ClrMethodTerm(MethodInfo method) =>
            this.method = method;

        public override Term HigherOrder =>
            GetMethodHigherOrder(this.method);

        object IValueTerm.Value =>
            this.method;

        public override Term Infer(InferContext context) =>
            this;

        Term IApplicableTerm.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
        {
            context.Unify(
                ((LambdaTerm)this.HigherOrder).Parameter,
                inferredArgumentHint.HigherOrder);
            context.Unify(
                ((LambdaTerm)this.HigherOrder).Body,
                higherOrderHint);

            return this;
        }

        public override Term Fixup(FixupContext context) =>
            this;

        Term IApplicableTerm.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        internal Term Invoke(IValueTerm argument) =>
            ClrConstantTerm.From(this.method.Invoke(null, new object[] { argument.Value }));

        AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
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

        private static LambdaTerm GetMethodHigherOrder(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return LambdaTerm.From(
                ClrTypeTerm.From((parameters.Length == 0) ? typeof(void) : parameters[0].ParameterType),
                ClrTypeTerm.From(method.ReturnType));
        }

        public static Term From(MethodInfo method) =>
            new ClrMethodTerm(method);   // TODO: generic method
    }
}
