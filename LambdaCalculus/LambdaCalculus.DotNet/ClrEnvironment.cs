namespace Favalon
{
    public sealed class ClrEnvironment : Environment
    {
        private ClrEnvironment(int iterations) :
            base(iterations)
        {
            this.SetBoundTerm("bool", ClrTermFactory.Type<bool>());
        }

        public static new ClrEnvironment Create(int iterations = DefaultIterations) =>
            new ClrEnvironment(iterations);
    }
}
