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

        public static Environment BindMutableTypeOperators(this Environment environment) =>
            environment.
                BindMutable("+", TypeSumOperatorTerm.Instance).
                BindMutable("*", TypeProductOperatorTerm.Instance);
    }
}
