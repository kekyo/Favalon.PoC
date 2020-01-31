using Favalon.Terms.Algebraic;

namespace Favalon.Terms.Types
{
    public class TypeProductTerm : ProductTerm
    {
        protected TypeProductTerm(Term lhs, Term rhs, Term higherOrder, TypeCalculator calculator) :
            base(lhs, rhs, higherOrder, calculator)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new TypeProductTerm(lhs, rhs, higherOrder,(TypeCalculator)this.Calculator);

        public static new TypeProductTerm Create(Term lhs, Term rhs, Term higherOrder) =>
            new TypeProductTerm(lhs, rhs, higherOrder, TypeCalculator.Instance);
    }
}
