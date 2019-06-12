namespace BasicSyntaxTree.Types.Unresolved
{
    public sealed class UnresolvedTypeConstructor : UnresolvedClrType
    {
        internal UnresolvedTypeConstructor(System.Type typeConstructor) : base(typeConstructor)
        {
        }

        public override bool IsResolved => false;

        public override string ToString() =>
            $"{this.Type.Name}<>";
    }
}
