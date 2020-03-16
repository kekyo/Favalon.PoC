using Favalon.Terms;
using Favalon.Terms.Algebraic;
using Favalon.Terms.Logical;
using Favalon.Terms.Operators;
using Favalon.Terms.Types;

namespace Favalon
{
    public static class EnvironmentExtension
    {
        public static Environment BindMutableBasisTerms(this Environment environment) =>
            environment.
                BindMutable("*", KindTerm.Instance).
                BindMutable("bool", BooleanTerm.Type).
                BindMutable("true", BooleanTerm.From(true)).
                BindMutable("false", BooleanTerm.From(false));

        public static Environment BindMutableAlgebraicOperators(this Environment environment) =>
            environment.
                BindMutable("+", SumOperatorTerm.Create(UnspecifiedTerm.Instance)).
                BindMutable("*", ProductOperatorTerm.Create(UnspecifiedTerm.Instance)).
                BindMutable(":>", WideningOperatorTerm.Create(UnspecifiedTerm.Instance, AlgebraicCalculator.Instance));
    }
}
