using Favalon.Terms.Contexts;

namespace Favalon.Terms.Types
{
    public sealed class ClrTypeProductOperatorTerm : Term, IApplicableTerm
    {
        private ClrTypeProductOperatorTerm()
        { }

        public override Term HigherOrder =>
            LambdaTerm.Kind2;

        public override Term Infer(InferContext context) =>
            this;

        Term IApplicableTerm.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
        {
            // (? -> inferredArgumentHint:? -> ?):higherOrderHint
            var higherOrderFromArgument = LambdaTerm.From(
                inferredArgumentHint.HigherOrder,
                LambdaTerm.From(inferredArgumentHint.HigherOrder, inferredArgumentHint.HigherOrder));

            context.Unify(higherOrderHint, LambdaTerm.Kind2);
            context.Unify(higherOrderFromArgument, LambdaTerm.Kind2);

            return this;
        }

        public override Term Fixup(FixupContext context) =>
            this;

        Term IApplicableTerm.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            AppliedResult.Applied(
                new ProductClosureTerm(argument),
                argument);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ClrTypeSumOperatorTerm;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "*";

        public static readonly ClrTypeProductOperatorTerm Instance =
            new ClrTypeProductOperatorTerm();

        private sealed class ProductClosureTerm : Term, IApplicableTerm
        {
            private readonly Term lhs;

            public ProductClosureTerm(Term lhs) =>
                this.lhs = lhs;

            public override Term HigherOrder =>
                LambdaTerm.Kind;

            public override Term Infer(InferContext context)
            {
                var lhs = this.lhs.Infer(context);

                context.Unify(lhs.HigherOrder, KindTerm.Instance);

                return
                    this.lhs.EqualsWithHigherOrder(lhs) ?
                        this :
                        new ProductClosureTerm(lhs);
            }

            Term IApplicableTerm.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
            {
                var lhs = this.lhs.Infer(context);

                // (inferredArgumentHint -> ?):higherOrderHint
                var higherOrderFromArgument = LambdaTerm.From(
                    lhs.HigherOrder,
                    inferredArgumentHint.HigherOrder);

                context.Unify(higherOrderHint, LambdaTerm.Kind);
                context.Unify(higherOrderFromArgument, LambdaTerm.Kind);

                return
                    this.lhs.EqualsWithHigherOrder(lhs) ?
                        this :
                        new ProductClosureTerm(lhs);
            }

            public override Term Fixup(FixupContext context)
            {
                var lhs = this.lhs.Fixup(context);

                return
                    this.lhs.EqualsWithHigherOrder(lhs)  ?
                        this :
                        new ProductClosureTerm(lhs);
            }

            Term IApplicableTerm.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint)
            {
                var lhs = this.lhs.Fixup(context);

                return
                    this.lhs.EqualsWithHigherOrder(lhs) ?
                        this :
                        new ProductClosureTerm(lhs);
            }

            public override Term Reduce(ReduceContext context)
            {
                var lhs = this.lhs.Reduce(context);

                return
                    this.lhs.EqualsWithHigherOrder(lhs)  ?
                        this :
                        new ProductClosureTerm(lhs);
            }

            AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint)
            {
                var lhs = this.lhs.Reduce(context);
                var rhs = argument.Reduce(context);

                // TODO: calculate product between types.
                if (ClrTypeCalculator.Instance.Widening(lhs, rhs) is Term applied1)
                {
                    return AppliedResult.Applied(applied1, rhs);
                }

                if (ClrTypeCalculator.Instance.Widening(rhs, lhs) is Term applied2)
                {
                    return AppliedResult.Applied(applied2, rhs);
                }

                return AppliedResult.Applied(ClrTypeProductTerm.Create(lhs, rhs), rhs);
            }

            protected override bool OnEquals(EqualsContext context, Term? other) =>
                other is ProductClosureTerm term ? this.lhs.Equals(term.lhs) : false;

            protected override string OnPrettyPrint(PrettyPrintContext context) =>
                $"* {this.lhs.PrettyPrint(context)}";

            public void Deconstruct(out Term lhs, out Term higherOrder)
            {
                lhs = this.lhs;
                higherOrder = this.HigherOrder;
            }
        }
    }
}
