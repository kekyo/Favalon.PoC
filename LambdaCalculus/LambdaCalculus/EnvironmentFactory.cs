namespace Favalon
{
    public static class EnvironmentFactory
    {
        public const int DefaultIterations = 1000;

        public static Environment Create(int iterations = DefaultIterations) =>
            Environment.Pure(iterations).
            BindMutableBasisTerms().
            BindMutableAlgebraicOperators();
    }
}
