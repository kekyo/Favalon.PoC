namespace Favalon
{
    public sealed class ClrEnvironment : Environment
    {
        private ClrEnvironment()
        {
            this.SetBoundTerm("bool", ClrTermFactory.Type<bool>());
        }

        public static new ClrEnvironment Create() =>
            new ClrEnvironment();
    }
}
