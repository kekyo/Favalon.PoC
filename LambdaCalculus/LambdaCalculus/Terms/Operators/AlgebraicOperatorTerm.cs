using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;

namespace Favalon.Terms.Operators
{
    public abstract class AlgebraicOperatorTerm<TCalculator> : Term, IApplicableTerm
        where TCalculator : AlgebraicCalculator
    {
        protected readonly TCalculator Calculator;

        protected AlgebraicOperatorTerm(TCalculator calculator) =>
            this.Calculator = calculator;

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
            this.ReduceForApply(context, argument, higherOrderHint);

        protected abstract AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint);
    }

    public abstract class AlgebraicOperatorTerm<TCalculator, TOperatorTerm> : AlgebraicOperatorTerm<TCalculator>
        where TCalculator : AlgebraicCalculator
        where TOperatorTerm : AlgebraicOperatorTerm<TCalculator>
    {
        protected AlgebraicOperatorTerm(TCalculator calculator) :
            base(calculator)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is TOperatorTerm;
    }

    public abstract class AlgebraicOperatorClosureTerm<TCalculator> : Term, IApplicableTerm
        where TCalculator : AlgebraicCalculator
    {
        protected readonly TCalculator Calculator;

        public readonly Term Lhs;

        protected AlgebraicOperatorClosureTerm(TCalculator calculator, Term lhs)
        {
            this.Calculator = calculator;
            this.Lhs = lhs;
        }

        public override Term HigherOrder =>
            LambdaTerm.Unspecified;

        protected abstract Term OnCreate(TCalculator calculator, Term lhs);

        public override sealed Term Infer(InferContext context)
        {
            var lhs = this.Lhs.Infer(context);

            var partiallyHigherOrder = LambdaTerm.From(
                lhs.HigherOrder,
                UnspecifiedTerm.Instance);

            var higherOrder = this.HigherOrder.Infer(context);

            context.Unify(partiallyHigherOrder, higherOrder);

            return
                this.Lhs.EqualsWithHigherOrder(lhs) ?
                    this :
                    this.OnCreate(this.Calculator, lhs);
        }

        Term IApplicableTerm.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
        {
            var lhs = this.Lhs.Infer(context);

            // (inferredArgumentHint -> ?):higherOrderHint
            var higherOrderFromArgument = LambdaTerm.From(
                lhs.HigherOrder,
                inferredArgumentHint.HigherOrder);

            var higherOrder = this.HigherOrder.Infer(context);

            context.Unify(higherOrderHint, higherOrder);
            context.Unify(higherOrderFromArgument, higherOrder);

            return
                this.Lhs.EqualsWithHigherOrder(lhs) ?
                    this :
                    this.OnCreate(this.Calculator, lhs);
        }

        public override sealed Term Fixup(FixupContext context)
        {
            var lhs = this.Lhs.Fixup(context);

            return
                this.Lhs.EqualsWithHigherOrder(lhs) ?
                    this :
                    this.OnCreate(this.Calculator, lhs);
        }

        Term IApplicableTerm.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint)
        {
            var lhs = this.Lhs.Fixup(context);

            return
                this.Lhs.EqualsWithHigherOrder(lhs) ?
                    this :
                    this.OnCreate(this.Calculator, lhs);
        }

        public override sealed Term Reduce(ReduceContext context)
        {
            var lhs = this.Lhs.Reduce(context);

            return
                this.Lhs.EqualsWithHigherOrder(lhs) ?
                    this :
                    this.OnCreate(this.Calculator, lhs);
        }

        AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            ReduceForApply(context, argument, higherOrderHint);

        protected abstract AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint);

        public void Deconstruct(out Term lhs, out Term higherOrder)
        {
            lhs = this.Lhs;
            higherOrder = this.HigherOrder;
        }
    }

    public abstract class AlgebraicOperatorClosureTerm<TCalculator, TClosureTerm> : AlgebraicOperatorClosureTerm<TCalculator>
        where TCalculator : AlgebraicCalculator
        where TClosureTerm : AlgebraicOperatorClosureTerm<TCalculator>
    {
        protected AlgebraicOperatorClosureTerm(TCalculator calculator, Term lhs) :
            base(calculator, lhs)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is TClosureTerm term ? this.Lhs.Equals(term.Lhs) : false;
    }
}
