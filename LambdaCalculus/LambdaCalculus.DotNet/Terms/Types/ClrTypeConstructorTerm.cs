using Favalon.Terms.Contexts;
using System;
using System.Reflection;

namespace Favalon.Terms.Types
{
    public sealed class ClrTypeConstructorTerm : Term, IApplicableTerm
    {
        private readonly Type type;

        internal ClrTypeConstructorTerm(Type type) =>
            this.type = type;

        public override Term HigherOrder =>
            LambdaTerm.Repeat(KindTerm.Instance, this.type.GetGenericArguments().Length + 1);

        public override Term Infer(InferContext context) =>
            this;

        Term IApplicableTerm.InferForApply(InferContext context, Term argument, Term appliedHigherOrderHint) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        Term IApplicableTerm.FixupForApply(FixupContext context, Term argument, Term appliedHigherOrderHint) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term appliedHigherOrderHint)
        {
            var argument_ = argument.Reduce(context);

            if (argument_ is ClrTypeTerm typeTerm)
            {
                var realType = type.MakeGenericType(typeTerm.Type);
                return AppliedResult.Applied(ClrTypeTerm.From(realType), argument_);
            }
            else
            {
                return AppliedResult.Ignored(this, argument_);
            }
        }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ClrTypeConstructorTerm rhs ? type.Equals(rhs.type) : false;

        public override int GetHashCode() =>
            type.GetHashCode();

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            type.PrettyPrint(context);
    }
}
