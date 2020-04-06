using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;

namespace Favalon.Terms.Operators
{
    public sealed class WideningOperatorTerm : BinaryOperatorTerm<WideningOperatorTerm>
    {
        private readonly AlgebraicCalculator calculator;

        private WideningOperatorTerm(Term higherOrder, AlgebraicCalculator calculator) :
            base(higherOrder) =>
            this.calculator = calculator;

        protected override Term OnCreate(Term higherOrder) =>
            new WideningOperatorTerm(higherOrder, calculator);

        protected override Term OnCreateClosure(Term lhs, Term higherOrder) =>
            new WideningOperatorClosureTerm(lhs, higherOrder, calculator);

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            ":>";

        public static WideningOperatorTerm Create(Term higherOrder, AlgebraicCalculator calculator) =>
            new WideningOperatorTerm(LambdaTerm.Repeat(higherOrder, 3)!, calculator);

        private sealed class WideningOperatorClosureTerm : ClosureTerm<WideningOperatorClosureTerm>
        {
            private readonly AlgebraicCalculator calculator;

            public WideningOperatorClosureTerm(Term lhs, Term higherOrder, AlgebraicCalculator calculator) :
                base(lhs, higherOrder) =>
                this.calculator = calculator;

            protected override Term OnCreate(Term lhs, Term higherOrder) =>
                new WideningOperatorClosureTerm(lhs, higherOrder, calculator);

            protected override Term OnCreateClosure(Term lhs, Term rhs, Term higherOrder) =>
                WideningTerm.Create(lhs, rhs, higherOrder, calculator);

            protected override string OnPrettyPrint(PrettyPrintContext context) =>
                $":> {this.Lhs.PrettyPrint(context)}";
        }
    }
}
