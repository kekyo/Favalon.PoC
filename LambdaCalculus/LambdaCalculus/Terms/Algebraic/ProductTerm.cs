using Favalon.Terms.Contexts;

namespace Favalon.Terms.Algebraic
{
    public abstract class ProductTerm : AlgebraicTerm<ProductTerm>
    {
        protected ProductTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} * {this.Rhs.PrettyPrint(context)}";
    }
}
