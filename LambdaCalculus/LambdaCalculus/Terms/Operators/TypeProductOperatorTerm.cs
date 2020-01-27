using Favalon.Terms.Contexts;
using Favalon.Terms.Types;

namespace Favalon.Terms.Operators
{
    public sealed class TypeProductOperatorTerm : ProductOperatorTerm<TypeCalculator>
    {
        private TypeProductOperatorTerm() :
            base(TypeCalculator.Instance)
        { }

        protected override AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            AppliedResult.Applied(
                new TypeProductOperatorClosureTerm(argument),
                argument);

        public static readonly TypeProductOperatorTerm Instance =
            new TypeProductOperatorTerm();
    }

    internal sealed class TypeProductOperatorClosureTerm : ProductOperatorClosureTerm<TypeCalculator>
    {
        public TypeProductOperatorClosureTerm(Term lhs) :
            base(TypeCalculator.Instance, lhs)
        { }

        protected override Term OnCreate(TypeCalculator calculator, Term lhs) =>
            new TypeProductOperatorClosureTerm(lhs);

        protected override Term OnCreateTerm(Term lhs, Term rhs, Term higherOrder) =>
            TypeProductTerm.Create(lhs, rhs, higherOrder);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is TypeProductOperatorClosureTerm term ? this.Lhs.Equals(term.Lhs) : false;
    }
}
