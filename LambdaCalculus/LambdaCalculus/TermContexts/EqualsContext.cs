namespace Favalon.TermContexts
{
    public sealed class EqualsContext
    {
        public readonly bool IncludeHigherOrder;

        internal EqualsContext(bool includeHigherOrder)
        {
            this.IncludeHigherOrder = includeHigherOrder;
        }
    }
}
