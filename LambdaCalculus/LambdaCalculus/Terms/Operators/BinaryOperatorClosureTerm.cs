using Favalon.Terms.Contexts;

namespace Favalon.Terms.Operators
{
    partial class BinaryOperatorTerm
    {
        protected abstract class ClosureTerm : Term, IApplicableTerm
        {
            public readonly Term Lhs;

            private protected ClosureTerm(Term lhs, Term higherOrder)
            {
                this.Lhs = lhs;
                this.HigherOrder = higherOrder;
            }

            public override Term HigherOrder { get; }

            protected abstract Term OnCreate(Term lhs, Term higherOrder);

            public override Term Infer(InferContext context)
            {
                var lhs = this.Lhs.Infer(context);
                var higherOrder = context.ResolveHigherOrder(this.HigherOrder);

                return
                    this.Lhs.EqualsWithHigherOrder(lhs) &&
                    this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                        this :
                        this.OnCreate(lhs, higherOrder);
            }

            public override Term Fixup(FixupContext context)
            {
                var lhs = this.Lhs.Fixup(context);
                var higherOrder = this.HigherOrder.Fixup(context);

                return
                    this.Lhs.EqualsWithHigherOrder(lhs) &&
                    this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                        this :
                        this.OnCreate(lhs, higherOrder);
            }

            public override Term Reduce(ReduceContext context)
            {
                var lhs = this.Lhs.Reduce(context);
                var higherOrder = this.HigherOrder.Reduce(context);

                return
                    this.Lhs.EqualsWithHigherOrder(lhs) &&
                    this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                        this :
                        this.OnCreate(lhs, higherOrder);
            }

            protected abstract Term OnCreateClosure(Term lhs, Term rhs, Term higherOrder);

            AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term appliedHigherOrderHint)
            {
                var lhs = this.Lhs.Reduce(context);
                var argument_ = argument.Reduce(context);
                var higherOrder = this.HigherOrder.Reduce(context);
                if (higherOrder is LambdaTerm(_, Term bodyHigherOrder))
                {
                    return AppliedResult.Applied(
                        this.OnCreateClosure(lhs, argument_, bodyHigherOrder), argument_);
                }
                else
                {
                    return this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                        AppliedResult.Ignored(this, argument_) :
                        AppliedResult.Applied(this.OnCreate(lhs, higherOrder), argument_);
                }
            }

            protected override string OnPrettyPrint(PrettyPrintContext context) =>
                $"+ {this.Lhs.PrettyPrint(context)}";
        }
    }

    partial class BinaryOperatorTerm<T>
    {
        protected abstract class ClosureTerm<U> : ClosureTerm
            where U : ClosureTerm
        {
            protected ClosureTerm(Term lhs, Term higherOrder) :
                base(lhs, higherOrder)
            { }

            protected override bool OnEquals(EqualsContext context, Term? other) =>
                other is U rhs && this.Lhs.Equals(rhs.Lhs);
        }
    }
}
