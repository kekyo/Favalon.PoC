namespace Favalet.Contexts
{
    public struct PrettyStringContext
    {
        public readonly bool IsSimple;
        internal readonly bool IsPartial;

        public PrettyStringContext(bool isSimple)
        {
            IsSimple = isSimple;
            IsPartial = false;
        }

        private PrettyStringContext(bool isSimple, bool isPartial)
        {
            IsSimple = isSimple;
            IsPartial = isPartial;
        }

        internal PrettyStringContext MakePartial() =>
            new PrettyStringContext(this.IsSimple, true);

        public static readonly PrettyStringContext Simple =
            new PrettyStringContext(true);
        public static readonly PrettyStringContext Strict =
            new PrettyStringContext(false);
    }
}
