using Favalon.Terms.Contexts;

namespace Favalon.Terms.Types
{
    public sealed class ClrTypeSumOperatorTerm : Term, IApplicable
    {
        private ClrTypeSumOperatorTerm()
        { }

        public override Term HigherOrder =>
            LambdaTerm.Kind2;

        public override Term Infer(InferContext context) =>
            this;

        Term IApplicable.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
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

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        AppliedResult IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            AppliedResult.Applied(
                new SumClosureTerm(argument),
                argument);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ClrTypeSumOperatorTerm;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "+";

        public static readonly ClrTypeSumOperatorTerm Instance =
            new ClrTypeSumOperatorTerm();

        private sealed class SumClosureTerm : Term, IApplicable
        {
            private readonly Term lhs;

            public SumClosureTerm(Term lhs) =>
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
                        new SumClosureTerm(lhs);
            }

            Term IApplicable.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
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
                        new SumClosureTerm(lhs);
            }

            public override Term Fixup(FixupContext context)
            {
                var lhs = this.lhs.Fixup(context);

                return
                    this.lhs.EqualsWithHigherOrder(lhs)  ?
                        this :
                        new SumClosureTerm(lhs);
            }

            Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint)
            {
                var lhs = this.lhs.Fixup(context);

                return
                    this.lhs.EqualsWithHigherOrder(lhs) ?
                        this :
                        new SumClosureTerm(lhs);
            }

            public override Term Reduce(ReduceContext context)
            {
                var lhs = this.lhs.Reduce(context);

                return
                    this.lhs.EqualsWithHigherOrder(lhs)  ?
                        this :
                        new SumClosureTerm(lhs);
            }

            AppliedResult IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint)
            {
                var lhs = this.lhs.Reduce(context);
                var rhs = argument.Reduce(context);

                if (ClrTypeCalculator.Instance.Widening(lhs, rhs) is Term applied1)
                {
                    return AppliedResult.Applied(applied1, rhs);
                }

                if (ClrTypeCalculator.Instance.Widening(rhs, lhs) is Term applied2)
                {
                    return AppliedResult.Applied(applied2, rhs);
                }

                return AppliedResult.Applied(ClrTypeSumTerm.Create(lhs, rhs), rhs);
            }

            protected override bool OnEquals(EqualsContext context, Term? other) =>
                other is SumClosureTerm term ? this.lhs.Equals(term.lhs) : false;

            protected override string OnPrettyPrint(PrettyPrintContext context) =>
                $"+ {this.lhs.PrettyPrint(context)}";

            public void Deconstruct(out Term lhs, out Term higherOrder)
            {
                lhs = this.lhs;
                higherOrder = this.HigherOrder;
            }
        }
    }
}
