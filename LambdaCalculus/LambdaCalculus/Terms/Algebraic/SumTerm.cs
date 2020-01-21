using Favalon.TermContexts;

namespace Favalon.Terms.Algebraic
{
    public sealed class SumTerm : AlgebraicTerm<SumTerm>
    {
        private SumTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new SumTerm(lhs, rhs, higherOrder);

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Lhs.PrettyPrint(context)} + {this.Rhs.PrettyPrint(context)}";

        public static SumTerm Create(Term lhs, Term rhs, Term higherOrder) =>
            new SumTerm(lhs, rhs, higherOrder);
    }
}
