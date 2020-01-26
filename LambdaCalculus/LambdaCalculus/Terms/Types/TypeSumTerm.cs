using Favalon.Terms.Algebraic;

namespace Favalon.Terms.Types
{
    public class TypeSumTerm : SumTerm
    {
        protected TypeSumTerm(Term lhs, Term rhs, Term higherOrder, TypeCalculator calculator) :
            base(lhs, rhs, higherOrder, calculator)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new TypeSumTerm(lhs, rhs, higherOrder, TypeCalculator.Instance);

        public static new TypeSumTerm Create(Term lhs, Term rhs, Term higherOrder) =>
            new TypeSumTerm(lhs, rhs, higherOrder, TypeCalculator.Instance);
    }
}
