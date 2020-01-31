using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;

namespace Favalon.Terms.Operators
{
    public abstract class ProductOperatorTerm<TCalculator> : AlgebraicOperatorTerm<TCalculator>
        where TCalculator : AlgebraicCalculator
    {
        protected ProductOperatorTerm(TCalculator calculator) :
            base(calculator)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ProductOperatorTerm<TCalculator>;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "*";
    }

    public abstract class ProductOperatorClosureTerm<TCalculator> : AlgebraicOperatorClosureTerm<TCalculator>
        where TCalculator : AlgebraicCalculator
    {
        protected ProductOperatorClosureTerm(TCalculator calculator, Term lhs) :
            base(calculator, lhs)
        { }

        protected abstract Term OnCreateTerm(Term lhs, Term rhs, Term higherOrder);

        protected override AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            ProductTerm.InternalReduce(
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
            $"* {this.Lhs.PrettyPrint(context)}";
    }

    public sealed class ProductOperatorTerm : ProductOperatorTerm<AlgebraicCalculator>
    {
        private ProductOperatorTerm() :
            base(AlgebraicCalculator.Instance)
        { }

        protected override AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            AppliedResult.Applied(
                new ProductOperatorClosureTerm(argument),
                argument);

        public static readonly ProductOperatorTerm Instance =
            new ProductOperatorTerm();
    }

    internal sealed class ProductOperatorClosureTerm : ProductOperatorClosureTerm<AlgebraicCalculator>
    {
        public ProductOperatorClosureTerm(Term lhs) :
            base(AlgebraicCalculator.Instance, lhs)
        { }

        protected override Term OnCreate(AlgebraicCalculator calculator, Term lhs) =>
            new ProductOperatorClosureTerm(lhs);

        protected override Term OnCreateTerm(Term lhs, Term rhs, Term higherOrder) =>
            ProductTerm.Create(lhs, rhs, higherOrder);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ProductOperatorClosureTerm term ? this.Lhs.Equals(term.Lhs) : false;
    }
}
