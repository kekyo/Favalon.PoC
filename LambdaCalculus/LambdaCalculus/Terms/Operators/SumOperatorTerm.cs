using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;

namespace Favalon.Terms.Operators
{
    public abstract class SumOperatorTerm<TCalculator> : AlgebraicOperatorTerm<TCalculator>
        where TCalculator : AlgebraicCalculator
    {
        protected SumOperatorTerm(TCalculator calculator) :
            base(calculator)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is SumOperatorTerm<TCalculator>;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "+";
    }

    public abstract class SumOperatorClosureTerm<TCalculator> : AlgebraicOperatorClosureTerm<TCalculator>
        where TCalculator : AlgebraicCalculator
    {
        protected SumOperatorClosureTerm(TCalculator calculator, Term lhs) :
            base(calculator, lhs)
        { }

        protected abstract Term OnCreateTerm(Term lhs, Term rhs, Term higherOrder);

        protected override AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            SumTerm.InternalReduce(
                context,
                this.Lhs,
                argument,
                this.HigherOrder,
                (term, rhs) => (term != null) ?
                    AppliedResult.Applied(term, rhs) :
                    AppliedResult.Ignored(this, rhs),
                this.Calculator,
                this.OnCreateTerm);

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"+ {this.Lhs.PrettyPrint(context)}";
    }

    public sealed class SumOperatorTerm : SumOperatorTerm<AlgebraicCalculator>
    {
        private SumOperatorTerm() :
            base(AlgebraicCalculator.Instance)
        { }

        protected override AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            AppliedResult.Applied(
                new SumOperatorClosureTerm(argument),
                argument);

        public static readonly SumOperatorTerm Instance =
            new SumOperatorTerm();
    }

    internal sealed class SumOperatorClosureTerm : SumOperatorClosureTerm<AlgebraicCalculator>
    {
        public SumOperatorClosureTerm(Term lhs) :
            base(AlgebraicCalculator.Instance, lhs)
        { }

        protected override Term OnCreate(AlgebraicCalculator calculator, Term lhs) =>
            new SumOperatorClosureTerm(lhs);

        protected override Term OnCreateTerm(Term lhs, Term rhs, Term higherOrder) =>
            SumTerm.Create(lhs, rhs, higherOrder);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is SumOperatorClosureTerm term ? this.Lhs.Equals(term.Lhs) : false;
    }
}
