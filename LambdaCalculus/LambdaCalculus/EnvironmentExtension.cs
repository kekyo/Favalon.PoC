using Favalon.Terms.Logical;
using Favalon.Terms.Operators;

namespace Favalon
{
    public static class EnvironmentExtension
    {
        public static Environment BindBooleanTerms(this Environment environment)
        {
            environment.BindMutable("bool", BooleanTerm.Type);
            environment.BindMutable("true", BooleanTerm.From(true));
            environment.BindMutable("false", BooleanTerm.From(false));

            return environment;
        }

        public static Environment BindTypeOperators(this Environment environment) =>
            environment.
                BindMutable("+", TypeSumOperatorTerm.Instance).
                BindMutable("*", TypeProductOperatorTerm.Instance);
    }
}
