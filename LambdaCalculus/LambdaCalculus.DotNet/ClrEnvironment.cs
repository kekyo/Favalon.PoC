namespace Favalon
{
    public sealed class ClrEnvironment : Environment
    {
        private ClrEnvironment(int defaultIterations) :
            base(defaultIterations)
        {
            this.SetBoundTerm("bool", ClrTermFactory.Type<bool>());
        }

        public static new ClrEnvironment Create(int defaultIterations = 10000) =>
            new ClrEnvironment(defaultIterations);
    }
}
