using Favalon.Terms.Logical;
using Favalon.Terms.Operators;

namespace Favalon
{
    public static class EnvironmentExtension
    {
        public static Environment BindBooleanTerms(this Environment environment)
        {
            environment.BindTerm("bool", BooleanTerm.Type);
            environment.BindTerm("true", BooleanTerm.From(true));
            environment.BindTerm("false", BooleanTerm.From(false));

            return environment;
        }

        public static Environment BindTypeOperators(this Environment environment) =>
            environment.
                BindTerm("+", TypeSumOperatorTerm.Instance).
                BindTerm("*", TypeProductOperatorTerm.Instance);
    }
}
