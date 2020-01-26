using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;
using Favalon.Terms.Types;

namespace Favalon.Terms.Operators
{
    public class SumOperatorTerm : Term, IApplicableTerm
    {
        private readonly AlgebraicCalculator calculator;

        protected SumOperatorTerm(AlgebraicCalculator calculator) =>
            this.calculator = calculator;

        public override Term HigherOrder =>
            LambdaTerm.Unspecified2;

        public override Term Infer(InferContext context) =>
            this;

        Term IApplicableTerm.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
        {
            // (? -> inferredArgumentHint:? -> ?):higherOrderHint
            var higherOrderFromArgument = LambdaTerm.From(
                inferredArgumentHint.HigherOrder,
                LambdaTerm.From(inferredArgumentHint.HigherOrder, inferredArgumentHint.HigherOrder));

            var higherOrder = this.HigherOrder.Infer(context);

            context.Unify(higherOrderHint, higherOrder);
            context.Unify(higherOrderFromArgument, higherOrder);

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
                new SumClosureTerm(calculator, argument),
                argument);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is SumOperatorTerm;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "+";

        public static readonly SumOperatorTerm Instance =
            new SumOperatorTerm(AlgebraicCalculator.Instance);

        private sealed class SumClosureTerm : Term, IApplicableTerm
        {
            private readonly AlgebraicCalculator calculator;
            private readonly Term lhs;

            public SumClosureTerm(AlgebraicCalculator calculator, Term lhs)
            {
                this.calculator = calculator;
                this.lhs = lhs;
            }

            public override Term HigherOrder =>
                LambdaTerm.Unspecified;

            public override Term Infer(InferContext context)
            {
                var lhs = this.lhs.Infer(context);

                context.Unify(lhs.HigherOrder, KindTerm.Instance);

                return
                    this.lhs.EqualsWithHigherOrder(lhs) ?
                        this :
                        new SumClosureTerm(calculator, lhs);
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
                        new SumClosureTerm(calculator, lhs);
            }

            public override Term Fixup(FixupContext context)
            {
                var lhs = this.lhs.Fixup(context);

                return
                    this.lhs.EqualsWithHigherOrder(lhs)  ?
                        this :
                        new SumClosureTerm(calculator, lhs);
            }

            Term IApplicableTerm.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint)
            {
                var lhs = this.lhs.Fixup(context);

                return
                    this.lhs.EqualsWithHigherOrder(lhs) ?
                        this :
                        new SumClosureTerm(calculator, lhs);
            }

            public override Term Reduce(ReduceContext context)
            {
                var lhs = this.lhs.Reduce(context);

                return
                    this.lhs.EqualsWithHigherOrder(lhs)  ?
                        this :
                        new SumClosureTerm(calculator, lhs);
            }

            AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
                TypeSumTerm.InternalReduce(
                    context,
                    this.lhs,
                    argument,
                    KindTerm.Instance,
                    (term, rhs) => (term != null) ?
                        AppliedResult.Applied(term, rhs) :
                        AppliedResult.Ignored(this, rhs),
                    TypeCalculator.Instance,
                    TypeSumTerm.Create);

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
