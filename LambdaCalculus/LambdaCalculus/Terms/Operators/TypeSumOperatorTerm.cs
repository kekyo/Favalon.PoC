using Favalon.Terms.Contexts;
using Favalon.Terms.Types;

namespace Favalon.Terms.Operators
{
    public sealed class TypeSumOperatorTerm : SumOperatorTerm<TypeCalculator>
    {
        private TypeSumOperatorTerm() :
            base(TypeCalculator.Instance)
        { }

        protected override AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            AppliedResult.Applied(
                new TypeSumOperatorClosureTerm(argument),
                argument);

        public static readonly TypeSumOperatorTerm Instance =
            new TypeSumOperatorTerm();
    }

    internal sealed class TypeSumOperatorClosureTerm : SumOperatorClosureTerm<TypeCalculator>
    {
        public TypeSumOperatorClosureTerm(Term lhs) :
            base(TypeCalculator.Instance, lhs)
        { }

        protected override Term OnCreate(TypeCalculator calculator, Term lhs) =>
            new TypeSumOperatorClosureTerm(lhs);

        protected override Term OnCreateTerm(Term lhs, Term rhs, Term higherOrder) =>
            TypeSumTerm.Create(lhs, rhs, higherOrder);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is TypeSumOperatorClosureTerm term ? this.Lhs.Equals(term.Lhs) : false;
    }
}
