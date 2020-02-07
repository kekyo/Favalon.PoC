using Favalon.Terms.Types;

namespace Favalon
{
    public static class EnvironmentFactory
    {
        public const int DefaultIterations = 1000;

        public static Environment Create(int iterations = DefaultIterations) =>
            Environment.Pure(TypeCalculator.Instance, iterations).
            BindMutableBasisTerms().
            BindMutableAlgebraicOperators();
    }
}
