using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;

namespace Favalon.Terms.Operators
{
    public sealed class SumOperatorTerm : BinaryOperatorTerm<SumOperatorTerm>
    {
        private SumOperatorTerm(Term higherOrder) :
            base(higherOrder)
        { }

        protected override Term OnCreate(Term higherOrder) =>
            new SumOperatorTerm(higherOrder);

        protected override Term OnCreateClosure(Term lhs, Term higherOrder) =>
            new SumOperatorClosureTerm(lhs, higherOrder);

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "+";

        public static SumOperatorTerm Create(Term higherOrder) =>
            new SumOperatorTerm(LambdaTerm.Repeat(higherOrder, 3));

        private sealed class SumOperatorClosureTerm : ClosureTerm<SumOperatorClosureTerm>
        {
            public SumOperatorClosureTerm(Term lhs, Term higherOrder) :
                base(lhs, higherOrder)
            { }

            protected override Term OnCreate(Term lhs, Term higherOrder) =>
                new SumOperatorClosureTerm(lhs, higherOrder);

            protected override Term OnCreateClosure(Term lhs, Term rhs, Term higherOrder) =>
                SumTerm.Create(new[] { lhs, rhs }, higherOrder);

            protected override string OnPrettyPrint(PrettyPrintContext context) =>
                $"+ {this.Lhs.PrettyPrint(context)}";
        }
    }
}
