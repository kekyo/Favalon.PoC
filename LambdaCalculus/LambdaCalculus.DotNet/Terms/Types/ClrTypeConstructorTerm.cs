using Favalon.Terms.Contexts;
using System;

namespace Favalon.Terms.Types
{
    public sealed class ClrTypeConstructorTerm : Term, IApplicableTerm
    {
        private readonly Type type;

        internal ClrTypeConstructorTerm(Type type) =>
            this.type = type;

        public override Term HigherOrder =>
            // * -> * (TODO: make nested kind lambda from flatten generic type arguments: * -> * -> * ...)
            LambdaTerm.Kind;

        public override Term Infer(InferContext context) =>
            this;

        Term IApplicableTerm.InferForApply(InferContext context, Term argument, Term higherOrderHint) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        Term IApplicableTerm.FixupForApply(FixupContext context, Term argument, Term higherOrderHint) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint)
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
