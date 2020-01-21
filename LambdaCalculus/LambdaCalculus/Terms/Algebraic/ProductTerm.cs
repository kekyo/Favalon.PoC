using Favalon.Terms.Contexts;

namespace Favalon.Terms.Algebraic
{
    public sealed class ProductTerm : AlgebraicTerm<ProductTerm>
    {
        private ProductTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new ProductTerm(lhs, rhs, higherOrder);

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} * {this.Rhs.PrettyPrint(context)}";

        public static ProductTerm Create(Term lhs, Term rhs, Term higherOrder) =>
            new ProductTerm(lhs, rhs, higherOrder);
    }
}
