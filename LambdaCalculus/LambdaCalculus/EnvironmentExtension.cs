using Favalon.Terms.Logical;

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
    }
}
