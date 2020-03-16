using Favalon.Terms.Contexts;

namespace Favalon.Terms.Operators
{
    public abstract partial class BinaryOperatorTerm : Term, IApplicableTerm
    {
        private protected BinaryOperatorTerm(Term higherOrder) =>
            this.HigherOrder = higherOrder;

        public override Term HigherOrder { get; }

        protected abstract Term OnCreate(Term higherOrder);

        public override Term Infer(InferContext context)
        {
            var higherOrder = context.ResolveHigherOrder(this.HigherOrder);

            return this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                this :
                this.OnCreate(higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var higherOrder = this.HigherOrder.Fixup(context);

            return this.HigherOrder.EqualsWithHigherOrder(higherOrder)?
                this :
                this.OnCreate(higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);

            return this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                this :
                this.OnCreate(higherOrder);
        }

        protected abstract Term OnCreateClosure(Term lhs, Term higherOrder);

        AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term appliedHigherOrderHint)
        {
            var argument_ = argument.Reduce(context);
            var higherOrder = this.HigherOrder.Reduce(context);
            if (higherOrder is LambdaTerm(_, Term bodyHigherOrder))
            {
                return AppliedResult.Applied(
                    this.OnCreateClosure(argument_, bodyHigherOrder), argument_);
            }
            else
            {
                return this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    AppliedResult.Ignored(this, argument_) :
                    AppliedResult.Applied(this.OnCreate(higherOrder), argument_);
            }
        }
    }

    public abstract partial class BinaryOperatorTerm<T> : BinaryOperatorTerm
        where T : BinaryOperatorTerm
    {
        protected BinaryOperatorTerm(Term higherOrder) :
            base(higherOrder)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is T;
    }
}
